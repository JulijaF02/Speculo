namespace Speculo.Application.Features.Events.Queries.GetMoodStats;

public record MoodStatsDto(
    double AverageScore,
    int BestScore,
    DateTimeOffset BestDay,
    int WorstScore,
    DateTimeOffset WorstDay,
    int TotalLogs,
    int Days
);