namespace Speculo.Application.Features.Events.Queries.GetRecentWorkouts;

public record WorkoutLogDto(
    Guid id,
    string Type,
    int Minutes,
    int Score,
    DateTimeOffset Timestamp
);
