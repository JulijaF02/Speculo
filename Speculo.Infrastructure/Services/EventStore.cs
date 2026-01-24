using System.Text.Json;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Common;
using Speculo.Domain.Entities;
using Speculo.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Speculo.Infrastructure.Services;

public class EventStore(ISpeculoDbContext context, ICurrentUserProvider currentUserProvider) : IEventStore
{
    private static readonly Dictionary<string, Type> _eventTypeRegistry = new()
    {
        {nameof(MoodLoggedEvent), typeof(MoodLoggedEvent)},
        {nameof(WorkoutLoggedEvent), typeof(WorkoutLoggedEvent)}
    };
    public async Task<Guid> SaveAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : IDomainEvent
    {
        var eventId = Guid.NewGuid();

        var dbEvent = new Event
        {
            Id = eventId,
            UserId = currentUserProvider.UserId ?? Guid.Empty,
            Type = domainEvent.GetType().Name,
            Timestamp = domainEvent.OccurredOn,
            Payload = JsonSerializer.Serialize(domainEvent)
        };

        context.Events.Add(dbEvent);
        await context.SaveChangesAsync(ct);

        return eventId;
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid userId, CancellationToken ct = default)
    {
        var dbEvents = await context.Events
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Timestamp)
            .AsNoTracking()
            .ToListAsync(ct);

        var domainEvents = new List<IDomainEvent>();

        foreach (var dbEvent in dbEvents)
        {
            if (_eventTypeRegistry.TryGetValue(dbEvent.Type, out Type? targetType))
            {
                var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(dbEvent.Payload, targetType);

                if (domainEvent != null)
                {
                    domainEvents.Add(domainEvent);
                }
            }
        }
        return domainEvents;
    }
}
