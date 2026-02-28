using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Contracts.Events;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogSleep;

public class LogSleepCommandHandler(
    IEventStore eventStore,
    ICurrentUserProvider currentUserProvider,
    IEventBus eventBus)
    : IRequestHandler<LogSleepCommand, Guid>
{
    public async Task<Guid> Handle(LogSleepCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId
             ?? throw new UnauthorizedAccessException();

        // 1. Save domain event to PostgreSQL
        var sleepEvent = new SleepLoggedEvent(
            UserId: userId,
            Hours: request.Hours,
            Quality: request.Quality,
            Notes: request.Notes
        );

        var eventId = await eventStore.SaveAsync(sleepEvent, ct);

        // 2. Publish integration event to Kafka
        var integrationEvent = new SleepLoggedIntegrationEvent(
            UserId: userId,
            Hours: request.Hours,
            Quality: request.Quality,
            Notes: request.Notes,
            LoggedAt: sleepEvent.OccurredOn
        );

        await eventBus.PublishAsync(integrationEvent, ct);

        return eventId;
    }
}
