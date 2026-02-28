namespace Speculo.Contracts.Events;

/// <summary>
/// Published by Tracking Service when a user logs a workout.
/// Consumed by Analytics Service to update MongoDB projections.
/// </summary>
public record WorkoutLoggedIntegrationEvent(
    Guid UserId,
    string WorkoutType,
    int Minutes,
    int Score,
    string? Notes,
    DateTimeOffset LoggedAt
) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    public string EventType => nameof(WorkoutLoggedIntegrationEvent);
}
