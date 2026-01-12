using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Index za bržu pretragu svih event-ova jednog korisnika
        builder.HasIndex(e => e.UserId);
        
        // Postgres specifičan tip za JSON podatke
        builder.Property(e => e.Payload).HasColumnType("jsonb");
        
        builder.Property(e => e.Type).IsRequired().HasMaxLength(100);
    }
}