using System;
using Speculo.Domain.Common;

namespace Speculo.Domain.Events;

public record MoneyLoggedEvent(
    Guid UserId,
    decimal Amount,
    TransactionType Type,
    string Category,
    string? Merchant = null,
    string? Notes = null
) : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}

public enum TransactionType
{
    Expense,
    Income
}
