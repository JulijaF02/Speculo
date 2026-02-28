using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Contracts.Events;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogMood;

public class LogMoodCommandHandler(
    IEventStore eventStore,
    ICurrentUserProvider currentUserProvider,
    IEventBus eventBus)
    : IRequestHandler<LogMoodCommand, Guid>
{
    public async Task<Guid> Handle(LogMoodCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId
             ?? throw new UnauthorizedAccessException();

        // 1. Save domain event to PostgreSQL (the source of truth)
        var moodEvent = new MoodLoggedEvent(
            UserId: userId,
            Score: request.Score,
            Notes: request.Notes
        );

        var eventId = await eventStore.SaveAsync(moodEvent, ct);

        // 2. Publish integration event to Kafka (notify other services)
        var integrationEvent = new MoodLoggedIntegrationEvent(
            UserId: userId,
            Score: request.Score,
            Notes: request.Notes,
            LoggedAt: moodEvent.OccurredOn
        );

        await eventBus.PublishAsync(integrationEvent, ct);

        return eventId;
    }
}
