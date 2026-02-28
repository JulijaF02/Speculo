namespace Speculo.Contracts.Events;

/// <summary>
/// Published by Tracking Service when a user logs a financial transaction.
/// Consumed by Analytics Service to update MongoDB projections.
/// </summary>
public record MoneyLoggedIntegrationEvent(
    Guid UserId,
    decimal Amount,
    string TransactionType,  // "Income" or "Expense"
    string Category,
    string? Merchant,
    string? Notes,
    DateTimeOffset LoggedAt
) : IIntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    public string EventType => nameof(MoneyLoggedIntegrationEvent);
}
