using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Commands.LogWorkout;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogWorkout;

public class LogWorkoutCommandHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
: IRequestHandler<LogWorkoutCommand, Guid>
{
    public async Task<Guid> Handle(LogWorkoutCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId
            ?? throw new UnauthorizedAccessException();

        var workoutEvent = new WorkoutLoggedEvent(
            UserId: userId,
            Type: request.Type,
            Minutes: request.Minutes,
            Score: request.Score,
            Notes: request.Notes
        );

        var eventId = await eventStore.SaveAsync(workoutEvent, ct);

        return eventId;
    }
}