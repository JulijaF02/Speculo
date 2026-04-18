using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Contracts.Events;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogWorkout;

public class LogWorkoutCommandHandler(
    IEventStore eventStore,
    ICurrentUserProvider currentUserProvider,
    IEventBus eventBus)
    : IRequestHandler<LogWorkoutCommand, Guid>
{
    public async Task<Guid> Handle(LogWorkoutCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId
            ?? throw new UnauthorizedAccessException();

        // 1. Save domain event to PostgreSQL
        var workoutEvent = new WorkoutLoggedEvent(
            UserId: userId,
            Type: request.Type,
            Minutes: request.Minutes,
            Score: request.Score,
            Notes: request.Notes
        );

        var eventId = await eventStore.SaveAsync(workoutEvent, ct);

        // 2. Publish integration event to Kafka
        var integrationEvent = new WorkoutLoggedIntegrationEvent(
            UserId: userId,
            WorkoutType: request.Type,
            Minutes: request.Minutes,
            Score: request.Score,
            Notes: request.Notes,
            LoggedAt: workoutEvent.OccurredOn
        );

        await eventBus.PublishAsync(integrationEvent, ct);

        return eventId;
    }
}
