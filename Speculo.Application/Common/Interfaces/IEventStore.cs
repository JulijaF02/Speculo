using Speculo.Domain.Common;

namespace Speculo.Application.Common.Interfaces;

public interface IEventStore
{
    Task<Guid> SaveAsync<TEvent>(TEvent domainEvent, CancellationToken ct = default)
        where TEvent : IDomainEvent;

    Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid userId, CancellationToken ct = default);
}