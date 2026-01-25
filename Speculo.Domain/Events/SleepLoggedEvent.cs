using System;
using Speculo.Domain.Common;

namespace Speculo.Domain.Events;

public record SleepLoggedEvent
(
    Guid UserId,
    decimal Hours,
    int Quality,
    string? Notes = null

) : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}