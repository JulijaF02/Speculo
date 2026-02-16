namespace Speculo.Application.Features.Events.Queries.GetRecentMoods;

public record MoodLogDto(
    Guid Id,
    int Score,
    DateTimeOffset Timestamp

);
