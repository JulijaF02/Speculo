using MediatR;

namespace Speculo.Application.Features.Events.Queries.GetMoodStats;

public record GetMoodStatsQuery(int Days) : IRequest<MoodStatsDto>;