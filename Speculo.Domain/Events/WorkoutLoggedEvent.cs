using System;
using Speculo.Domain.Common;

namespace Speculo.Domain.Events;

public record WorkoutLoggedEvent
(
    Guid UserId,
    string Type,       // e.g., "Gym", "Run", "Yoga", "Swim"
    int Minutes,
    int Score,         // how satisfied was I with the workout
    string? Notes = null
) : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
