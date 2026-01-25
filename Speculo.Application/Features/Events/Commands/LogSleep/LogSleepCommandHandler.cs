using MediatR;
using Microsoft.VisualBasic;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogSleep;

public class LogSleepCommandHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
: IRequestHandler<LogSleepCommand, Guid>
{
    public async Task<Guid> Handle(LogSleepCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId
             ?? throw new UnauthorizedAccessException();

        var sleepEvent = new SleepLoggedEvent(
            UserId: userId,
            Hours: request.Hours,
            Quality: request.Quality,
            Notes: request.Notes
        );

        var eventId = await eventStore.SaveAsync(sleepEvent, ct);
        return eventId;
    }
}
