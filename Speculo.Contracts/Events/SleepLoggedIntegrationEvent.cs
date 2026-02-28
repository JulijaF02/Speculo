namespace Speculo.Contracts.Events;

/// <summary>
/// Published by Tracking Service when a user logs a sleep entry.
/// Consumed by Analytics Service to update MongoDB projections.
/// </summary>
public record SleepLoggedIntegrationEvent(
    Guid UserId,
    decimal Hours,
    int Quality,
    string? Notes,
    DateTimeOffset LoggedAt
) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    public string EventType => nameof(SleepLoggedIntegrationEvent);
}
