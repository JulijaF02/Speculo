namespace Speculo.Application.Features.Events.Queries.GetRecentWorkouts;

public record WorkoutLogDto(
    Guid Id,
    string Type,
    int Minutes,
    int Score,
    DateTimeOffset Timestamp
);
