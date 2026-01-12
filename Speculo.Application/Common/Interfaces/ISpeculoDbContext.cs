using Microsoft.EntityFrameworkCore;
using Speculo.Domain.Entities;

namespace Speculo.Application.Common.Interfaces;

public interface ISpeculoDbContext
{
    DbSet<User> Users { get; }
    DbSet<Event> Events { get; }
    DbSet<DailyAggregate> DailyAggregates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}