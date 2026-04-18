using MediatR;

namespace Speculo.Application.Features.Events.Commands.LogSleep;

public record LogSleepCommand(decimal Hours, int Quality, string? Notes = null) : IRequest<Guid>;