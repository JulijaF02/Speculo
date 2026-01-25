using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;
namespace Speculo.Application.Features.Events.Queries.GetRecentMoods;

public class GetRecentMoodQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider) :
IRequestHandler<GetRecentMoodQuery, IEnumerable<MoodLogDto>>
{
    public async Task<IEnumerable<MoodLogDto>> Handle(GetRecentMoodQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserProvider.UserId
             ?? throw new UnauthorizedAccessException();

        var events = await eventStore.GetEventsAsync(userId, cancellationToken);
        return events.OfType<MoodLoggedEvent>()
                     .Select(e => new MoodLogDto(e.Id, e.Score, e.OccurredOn));


    }
}