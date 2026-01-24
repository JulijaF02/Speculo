namespace Speculo.Application.Features.Events.Queries.GetRecentMoods;

public record MoodLogDto(
    Guid id,
    int score,
    DateTimeOffset Timestamp

);