using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        // Index for faster searching of all events for a single user
        builder.HasIndex(e => e.UserId);

        // Postgres-specific type for JSON data
        builder.Property(e => e.Payload).HasColumnType("jsonb");

        builder.Property(e => e.Type).IsRequired().HasMaxLength(100);
    }
}