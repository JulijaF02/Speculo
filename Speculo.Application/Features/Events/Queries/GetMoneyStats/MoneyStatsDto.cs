namespace Speculo.Application.Features.Events.Queries.GetMoneyStats;

public record MoneyStatsDto(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetSavings,
    int TotalTransactions,
    int Days
);
