namespace Speculo.Contracts.Events;

/// <summary>
/// Marker interface for events that cross service boundaries via Kafka.
/// Unlike domain events (internal to a service), integration events are
/// the "public API" of a microservice — other services consume these.
/// </summary>
public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
    string EventType { get; }
}
