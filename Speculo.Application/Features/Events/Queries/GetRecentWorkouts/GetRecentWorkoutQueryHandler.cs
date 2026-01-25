using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;
namespace Speculo.Application.Features.Events.Queries.GetRecentWorkouts;

public class GetRecentWorkoutQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
: IRequestHandler<GetRecentWorkoutQuery, IEnumerable<WorkoutLogDto>>
{
    public async Task<IEnumerable<WorkoutLogDto>> Handle(GetRecentWorkoutQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserProvider.UserId
             ?? throw new UnauthorizedAccessException();

        var events = await eventStore.GetEventsAsync(userId, cancellationToken);
        return events.OfType<WorkoutLoggedEvent>().Select(e => new WorkoutLogDto(e.Id, e.Type, e.Minutes, e.Score, e.OccurredOn));



    }
}
