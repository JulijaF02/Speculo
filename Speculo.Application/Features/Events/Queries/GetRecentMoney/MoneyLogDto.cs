using Speculo.Domain.Events;
namespace Speculo.Application.Features.Events.Queries.GetRecentMoney;

public record MoneyLogDto(
    Guid Id,
    decimal Amount,
    TransactionType Type,
    string Category,
    string? Merchant,
    string? Notes,
    DateTimeOffset Timestamp
);
