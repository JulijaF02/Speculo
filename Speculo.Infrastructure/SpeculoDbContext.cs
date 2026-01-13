using Microsoft.EntityFrameworkCore;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure;

public class SpeculoDbContext(DbContextOptions<SpeculoDbContext> options)
    : DbContext(options), ISpeculoDbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<DailyAggregate> DailyAggregates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This automatically finds all our 'IEntityTypeConfiguration' classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpeculoDbContext).Assembly);
    }
}