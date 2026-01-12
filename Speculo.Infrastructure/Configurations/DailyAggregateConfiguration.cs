using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure.Configurations;

public class DailyAggregateConfiguration : IEntityTypeConfiguration<DailyAggregate>
{
    public void Configure(EntityTypeBuilder<DailyAggregate> builder)
    {
        // Kombinovani ključ: ne možemo imati dva summary-ja za istog korisnika na isti dan
        builder.HasKey(a => new { a.UserId, a.Date });

        // Osiguravamo preciznost za novac
        builder.Property(a => a.TotalSpent).HasPrecision(18, 2);
    }
}