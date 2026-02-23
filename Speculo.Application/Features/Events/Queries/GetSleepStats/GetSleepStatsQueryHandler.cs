using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Queries.GetSleepStats;

public class GetSleepStatsQueryHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<GetSleepStatsQuery, SleepStatsDto>
{
    public async Task<SleepStatsDto> Handle(GetSleepStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserProvider.UserId ?? throw new UnauthorizedAccessException();
        var events = await eventStore.GetEventsAsync(userId, cancellationToken);

        var cutoff = DateTimeOffset.UtcNow.AddDays(-request.Days);

        var sleepEvents = events
            .OfType<SleepLoggedEvent>()
            .Where(e => e.OccurredOn >= cutoff)
            .ToList();

        if (sleepEvents.Count == 0)
            return new SleepStatsDto(0, 0, 0, 0, 0, request.Days);

        return new SleepStatsDto(
            AverageHours: (double)sleepEvents.Average(e => e.Hours),
            AverageQuality: sleepEvents.Average(e => e.Quality),
            BestQuality: sleepEvents.Max(e => e.Quality),
            WorstQuality: sleepEvents.Min(e => e.Quality),
            TotalLogs: sleepEvents.Count,
            Days: request.Days
        );
    }
}
