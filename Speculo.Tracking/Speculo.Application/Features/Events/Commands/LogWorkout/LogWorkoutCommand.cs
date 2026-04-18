using MediatR;
namespace Speculo.Application.Features.Events.Commands.LogWorkout;

public record LogWorkoutCommand(string Type, int Minutes, int Score, string? Notes = null) : IRequest<Guid>;