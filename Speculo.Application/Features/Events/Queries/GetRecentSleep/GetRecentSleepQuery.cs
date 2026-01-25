using MediatR;
namespace Speculo.Application.Features.Events.Queries.GetRecentSleep;

public record GetRecentSleepQuery : IRequest<IEnumerable<SleepLogDto>>;