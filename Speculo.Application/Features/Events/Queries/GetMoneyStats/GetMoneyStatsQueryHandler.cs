using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Queries.GetMoneyStats;

public class GetMoneyStatsQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<GetMoneyStatsQuery, MoneyStatsDto>
{
    public async Task<MoneyStatsDto> Handle(GetMoneyStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserProvider.UserId ?? throw new UnauthorizedAccessException();
        var cutoff = DateTimeOffset.UtcNow.AddDays(-request.Days);

        var events = await eventStore.GetEventsAsync(userId, cancellationToken, from: cutoff);


        var moneyEvents = events
            .OfType<MoneyLoggedEvent>()
            .ToList();

        if (moneyEvents.Count == 0)
            return new MoneyStatsDto(0, 0, 0, 0, request.Days);

        // Filter by enum value to split income vs expenses into separate totals
        var totalIncome = moneyEvents.Where(e => e.Type == TransactionType.Income).Sum(e => e.Amount);
        var totalExpenses = moneyEvents.Where(e => e.Type == TransactionType.Expense).Sum(e => e.Amount);

        return new MoneyStatsDto(
            TotalIncome: totalIncome,
            TotalExpenses: totalExpenses,
            NetSavings: totalIncome - totalExpenses,
            TotalTransactions: moneyEvents.Count,
            Days: request.Days
        );
    }
}
