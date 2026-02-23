using MediatR;

namespace Speculo.Application.Features.Events.Queries.GetMoneyStats;

public record GetMoneyStatsQuery(int Days) : IRequest<MoneyStatsDto>;
