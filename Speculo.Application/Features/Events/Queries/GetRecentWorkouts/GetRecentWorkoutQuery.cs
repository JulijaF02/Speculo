using MediatR;
namespace Speculo.Application.Features.Events.Queries.GetRecentWorkouts;

public record GetRecentWorkoutQuery : IRequest<IEnumerable<WorkoutLogDto>>;