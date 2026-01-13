using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogMood;

public class LogMoodCommandHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<LogMoodCommand, Guid>
{
    public async Task<Guid> Handle(LogMoodCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId;
        // 1. Unpack the Ticket and create a new Recipe (Event)
        var moodEvent = new MoodLoggedEvent(
            UserId: userId ?? Guid.Empty, // 
            Score: request.Score,
            Notes: request.Notes
        );

        // 2. Hand it to the Tool (Infrastructure) to save
        var eventId = await eventStore.SaveAsync(moodEvent, ct);

        // 3. Return the ID of the new event
        return eventId;
    }
}