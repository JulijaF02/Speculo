namespace Speculo.Application.Features.Events.Queries.GetSleepStats;

public record SleepStatsDto(
    double AverageHours,
    double AverageQuality,
    int BestQuality,
    int WorstQuality,
    int TotalLogs,
    int Days
);
