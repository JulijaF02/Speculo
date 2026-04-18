using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Speculo.Application.Common.Interfaces;
using Speculo.Contracts.Events;

namespace Speculo.Infrastructure.Messaging;

/// <summary>
/// Kafka implementation of IEventBus.
/// Serializes integration events to JSON and publishes them to a Kafka topic.
///
/// Key Kafka concepts used here:
/// - Producer: sends messages to a topic
/// - Topic: named channel (like "tracking-events")
/// - Key: we use UserId so all events for the same user go to the same partition
///   (this preserves ordering per user — mood logged before workout logged stays in that order)
/// </summary>
public class KafkaEventBus : IEventBus, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventBus> _logger;
    private const string DefaultTopic = "speculo-events";

    public KafkaEventBus(IConfiguration configuration, ILogger<KafkaEventBus> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",

            // Acknowledgment: wait for the Kafka leader broker to confirm receipt.
            // "Leader" means the primary replica acknowledged the write.
            // Options: 0 (fire and forget), 1 (leader ack), -1 (all replicas ack)
            Acks = Acks.Leader,

            // Retry on transient failures (network blips, broker restarts)
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 100
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        var message = new Message<string, string>
        {
            // Key = event type name — Kafka uses this for partitioning
            // All events of the same type go to the same partition (preserves ordering)
            Key = @event.EventType,

            // Value = the actual event data serialized as JSON
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        // Add event metadata as Kafka headers (useful for consumers to filter/route)
        message.Headers = new Headers
        {
            { "event-type", System.Text.Encoding.UTF8.GetBytes(@event.EventType) },
            { "event-id", System.Text.Encoding.UTF8.GetBytes(@event.Id.ToString()) },
            { "correlation-id", System.Text.Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) }
        };

        try
        {
            var result = await _producer.ProduceAsync(DefaultTopic, message, cancellationToken);

            _logger.LogInformation(
                "Published {EventType} to Kafka topic {Topic} [partition: {Partition}, offset: {Offset}]",
                @event.EventType,
                result.Topic,
                result.Partition.Value,
                result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex,
                "Failed to publish {EventType} to Kafka: {Error}",
                @event.EventType,
                ex.Error.Reason);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(5));
        _producer?.Dispose();
    }
}
