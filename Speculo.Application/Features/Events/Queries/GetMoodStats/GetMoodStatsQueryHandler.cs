using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Queries.GetMoodStats;

public class GetMoodStatsQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<GetMoodStatsQuery, MoodStatsDto>
{
    public async Task<MoodStatsDto> Handle(GetMoodStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserProvider.UserId ?? throw new UnauthorizedAccessException();
        var cutoff = DateTimeOffset.UtcNow.AddDays(-request.Days);

        var events = await eventStore.GetEventsAsync(userId, cancellationToken, from: cutoff);


        var moodEvents = events
            .OfType<MoodLoggedEvent>()
            .ToList();

        if (moodEvents.Count == 0)
            return new MoodStatsDto(0, 0, DateTimeOffset.MinValue, 0, DateTimeOffset.MinValue, 0, request.Days);

        var best = moodEvents.MaxBy(e => e.Score)!;
        var worst = moodEvents.MinBy(e => e.Score)!;

        return new MoodStatsDto(
            AverageScore: moodEvents.Average(e => e.Score),
            BestScore: best.Score,
            BestDay: best.OccurredOn,
            WorstScore: worst.Score,
            WorstDay: worst.OccurredOn,
            TotalLogs: moodEvents.Count,
            Days: request.Days
        );
    }
}