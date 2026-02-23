using MediatR;

namespace Speculo.Application.Features.Events.Queries.GetWorkoutStats;

public record GetWorkoutStatsQuery(int Days) : IRequest<WorkoutStatsDto>;
