namespace Speculo.Application.Features.Events.Queries.GetRecentSleep;

public record SleepLogDto(

    Guid Id,
    decimal Hours,
    int Quality,
    DateTimeOffset Timestamp

);