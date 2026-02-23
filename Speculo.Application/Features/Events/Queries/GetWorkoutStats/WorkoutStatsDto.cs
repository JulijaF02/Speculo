namespace Speculo.Application.Features.Events.Queries.GetWorkoutStats;

public record WorkoutStatsDto(
    int TotalWorkouts,
    int TotalMinutes,
    double AverageMinutes,
    double AverageScore,
    string MostCommonType,
    int Days
);
