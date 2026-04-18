using System;
using Speculo.Domain.Common;

namespace Speculo.Domain.Events;

//Represents that a user has logged their current mood 

public record MoodLoggedEvent(
    Guid UserId,
    int Score,
    string? Notes = null
) : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}