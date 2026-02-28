using Microsoft.EntityFrameworkCore;
using Speculo.Identity.Models;

namespace Speculo.Identity.Data;

/// <summary>
/// Identity Service has its OWN database context — separate from the Tracking Service.
/// This is key to microservices: each service owns its data.
/// No shared database = no coupling between services.
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(256);
        });
    }
}
