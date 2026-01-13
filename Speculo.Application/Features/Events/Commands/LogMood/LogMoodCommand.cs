
using MediatR;

namespace Speculo.Application.Features.Events.Commands.LogMood;

public record LogMoodCommand(int Score, string? Notes = null) : IRequest<Guid>;