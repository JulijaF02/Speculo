using MediatR;

namespace Speculo.Application.Features.Events.Queries.GetSleepStats;

public record GetSleepStatsQuery(int Days) : IRequest<SleepStatsDto>;
