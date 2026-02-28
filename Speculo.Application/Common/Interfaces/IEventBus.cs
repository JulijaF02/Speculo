using Speculo.Contracts.Events;

namespace Speculo.Application.Common.Interfaces;

/// <summary>
/// Abstraction for publishing integration events to a message broker.
/// The Application layer calls this interface — it doesn't know or care
/// whether the implementation uses Kafka, RabbitMQ, or anything else.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an integration event to the message broker.
    /// </summary>
    /// <param name="event">The integration event to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
}
