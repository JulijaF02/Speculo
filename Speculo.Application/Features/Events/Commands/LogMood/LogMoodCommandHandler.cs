using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogMood;

public class LogMoodCommandHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<LogMoodCommand, Guid>
{
    public async Task<Guid> Handle(LogMoodCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId
             ?? throw new UnauthorizedAccessException();

        var moodEvent = new MoodLoggedEvent(
            UserId: userId,
            Score: request.Score,
            Notes: request.Notes
        );

        var eventId = await eventStore.SaveAsync(moodEvent, ct);

        return eventId;
    }
}