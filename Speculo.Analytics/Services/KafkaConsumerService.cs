using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Speculo.Contracts.Events;
using StackExchange.Redis;

namespace Speculo.Analytics.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly ProjectionService _projectionService;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConfiguration _configuration;

    public KafkaConsumerService(
        ProjectionService projectionService,
        IConnectionMultiplexer redis,
        ILogger<KafkaConsumerService> logger,
        IConfiguration configuration)
    {
        _projectionService = projectionService;
        _redis = redis;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);

        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",

            GroupId = "analytics-service",

            AutoOffsetReset = AutoOffsetReset.Earliest,

            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe("speculo-events");
        _logger.LogInformation("Kafka consumer started, subscribed to 'speculo-events'");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(TimeSpan.FromMilliseconds(100));
                if (result == null)
                    continue;

                // Idempotency: extract event-id from Kafka header
                var eventIdHeader = result.Message.Headers
                    .FirstOrDefault(h => h.Key == "event-id");

                if (eventIdHeader != null)
                {
                    var eventIdStr = Encoding.UTF8.GetString(eventIdHeader.GetValueBytes());
                    if (Guid.TryParse(eventIdStr, out var eventId))
                    {
                        // Skip if already processed (at-least-once delivery guard)
                        if (await _projectionService.IsEventProcessedAsync(eventId))
                        {
                            _logger.LogInformation("Skipping duplicate event {EventId}", eventId);
                            consumer.Commit(result);
                            continue;
                        }
                    }
                }

                // Resolve event type from header or message key
                var eventTypeHeader = result.Message.Headers
                    .FirstOrDefault(h => h.Key == "event-type");
                var eventType = eventTypeHeader != null
                    ? Encoding.UTF8.GetString(eventTypeHeader.GetValueBytes())
                    : result.Message.Key;

                // Correlation ID for distributed tracing
                var correlationHeader = result.Message.Headers
                    .FirstOrDefault(h => h.Key == "correlation-id");
                var correlationId = correlationHeader != null
                    ? Encoding.UTF8.GetString(correlationHeader.GetValueBytes())
                    : "unknown";

                _logger.LogInformation(
                    "Processing event {EventType} [correlation: {CorrelationId}]",
                    eventType, correlationId);

                await DispatchEventAsync(eventType, result.Message.Value);

                // Mark as processed (idempotency)
                if (eventIdHeader != null)
                {
                    var eventIdStr = Encoding.UTF8.GetString(eventIdHeader.GetValueBytes());
                    if (Guid.TryParse(eventIdStr, out var eventId))
                    {
                        await _projectionService.MarkEventProcessedAsync(eventId, eventType);
                    }
                }

                // Invalidate Redis cache for the affected user
                await InvalidateCacheAsync(eventType, result.Message.Value);

                // Commit offset after full processing — ensures at-least-once delivery
                consumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Kafka message");
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
    }

    private async Task DispatchEventAsync(string eventType, string json)
    {
        switch (eventType)
        {
            case nameof(MoodLoggedIntegrationEvent):
                var mood = JsonSerializer.Deserialize<MoodLoggedIntegrationEvent>(json);
                if (mood != null)
                    await _projectionService.ApplyMoodLoggedAsync(mood.UserId, mood.Score);
                break;

            case nameof(SleepLoggedIntegrationEvent):
                var sleep = JsonSerializer.Deserialize<SleepLoggedIntegrationEvent>(json);
                if (sleep != null)
                    await _projectionService.ApplySleepLoggedAsync(sleep.UserId, sleep.Hours, sleep.Quality);
                break;

            case nameof(MoneyLoggedIntegrationEvent):
                var money = JsonSerializer.Deserialize<MoneyLoggedIntegrationEvent>(json);
                if (money != null)
                    await _projectionService.ApplyMoneyLoggedAsync(money.UserId, money.Amount, money.TransactionType);
                break;

            case nameof(WorkoutLoggedIntegrationEvent):
                var workout = JsonSerializer.Deserialize<WorkoutLoggedIntegrationEvent>(json);
                if (workout != null)
                    await _projectionService.ApplyWorkoutLoggedAsync(workout.UserId, workout.Minutes, workout.Score);
                break;

            default:
                _logger.LogWarning("Unknown event type: {EventType}", eventType);
                break;
        }
    }

    /// <summary>
    /// Invalidates the Redis cache for the affected user after a projection update.
    /// Ensures dashboard reads always reflect the latest processed events.
    /// </summary>
    private async Task InvalidateCacheAsync(string eventType, string json)
    {
        try
        {
            // Parse the JSON to get the UserId — all events have a UserId field
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("UserId", out var userIdElement))
            {
                var userId = userIdElement.GetString();
                if (userId != null)
                {
                    var db = _redis.GetDatabase();
                    await db.KeyDeleteAsync($"dashboard:{userId}");
                    _logger.LogDebug("Cache invalidated for user {UserId}", userId);
                }
            }
        }
        catch (Exception ex)
        {
            // Cache invalidation failure shouldn't kill the consumer
            _logger.LogWarning(ex, "Failed to invalidate cache");
        }
    }
}
