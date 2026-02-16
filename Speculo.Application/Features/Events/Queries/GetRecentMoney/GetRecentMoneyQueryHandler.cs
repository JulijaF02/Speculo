using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Queries.GetRecentMoney;

public class GetRecentMoneyQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<GetRecentMoneyQuery, IEnumerable<MoneyLogDto>>
{
    public async Task<IEnumerable<MoneyLogDto>> Handle(GetRecentMoneyQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserProvider.UserId ?? throw new UnauthorizedAccessException();
        var events = await eventStore.GetEventsAsync(userId, cancellationToken);

        return events
            .OfType<MoneyLoggedEvent>()
            .Select(e => new MoneyLogDto(
                e.Id,
                e.Amount,
                e.Type,
                e.Category,
                e.Merchant,
                e.Notes,
                e.OccurredOn
            ));
    }
}
