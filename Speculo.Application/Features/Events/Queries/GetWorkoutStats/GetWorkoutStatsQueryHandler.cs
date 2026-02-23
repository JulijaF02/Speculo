using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Queries.GetWorkoutStats;

public class GetWorkoutStatsQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<GetWorkoutStatsQuery, WorkoutStatsDto>
{
    public async Task<WorkoutStatsDto> Handle(GetWorkoutStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserProvider.UserId ?? throw new UnauthorizedAccessException();
        var events = await eventStore.GetEventsAsync(userId, cancellationToken);

        var cutoff = DateTimeOffset.UtcNow.AddDays(-request.Days);

        var workoutEvents = events
            .OfType<WorkoutLoggedEvent>()
            .Where(e => e.OccurredOn >= cutoff)
            .ToList();

        if (workoutEvents.Count == 0)
            return new WorkoutStatsDto(0, 0, 0, 0, "N/A", request.Days);

        // GroupBy groups all events by their Type value, then we pick
        // the group with the most entries — that's the most common workout type
        var mostCommonType = workoutEvents
            .GroupBy(e => e.Type)
            .MaxBy(g => g.Count())!.Key;

        return new WorkoutStatsDto(
            TotalWorkouts: workoutEvents.Count,
            TotalMinutes: workoutEvents.Sum(e => e.Minutes),
            AverageMinutes: workoutEvents.Average(e => e.Minutes),
            AverageScore: workoutEvents.Average(e => e.Score),
            MostCommonType: mostCommonType,
            Days: request.Days
        );
    }
}
