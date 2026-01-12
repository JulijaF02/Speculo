using Microsoft.EntityFrameworkCore;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure;

public class SpeculoDbContext : DbContext, ISpeculoDbContext
{
    public SpeculoDbContext(DbContextOptions<SpeculoDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<DailyAggregate> DailyAggregates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpeculoDbContext).Assembly);
    }
}