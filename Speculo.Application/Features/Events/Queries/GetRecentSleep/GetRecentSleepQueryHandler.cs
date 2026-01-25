using MediatR;
using Microsoft.VisualBasic;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;
namespace Speculo.Application.Features.Events.Queries.GetRecentSleep;

public class GetRecentSleepQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
: IRequestHandler<GetRecentSleepQuery, IEnumerable<SleepLogDto>>
{
    public async Task<IEnumerable<SleepLogDto>> Handle(GetRecentSleepQuery request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId
             ?? throw new UnauthorizedAccessException();

        var events = await eventStore.GetEventsAsync(userId, ct);
        return events.OfType<SleepLoggedEvent>().Select(e => new SleepLogDto(e.Id, e.Hours, e.Quality, e.OccurredOn));
    }
}
