namespace Speculo.Contracts.Events;

/// <summary>
/// Published by Identity Service when a new user registers.
/// Consumed by Tracking Service to know about available users.
/// </summary>
public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    DateTimeOffset RegisteredAt
) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    public string EventType => nameof(UserRegisteredEvent);
}
