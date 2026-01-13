using System.Text.Json;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Common;
using Speculo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Speculo.Infrastructure.Services;

public class EventStore(ISpeculoDbContext context, ICurrentUserProvider currentUserProvider) : IEventStore
{
    public async Task<Guid> SaveAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : IDomainEvent
    {
        var eventId = Guid.NewGuid();
        // 1. Create the database entity
        var dbEvent = new Event
        {
            Id = Guid.NewGuid(),
            UserId = currentUserProvider.UserId ?? Guid.Empty, // TODO: We will get this from the logged-in user soon!
            Type = domainEvent.GetType().Name,
            Timestamp = domainEvent.OccurredOn,

            // 2. Serialize the C# object (like MoodLoggedEvent) into JSON
            Payload = JsonSerializer.Serialize(domainEvent)
        };

        // 3. Add to the context and save
        context.Events.Add(dbEvent);
        await context.SaveChangesAsync(ct);
        return eventId;
    }

    public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid userId, CancellationToken ct = default)
    {
        // This is a placeholder - retrieving events is more complex because 
        // we have to "Deserialize" them back into their specific types!
        return await Task.FromResult(new List<IDomainEvent>());
    }
}