using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure.Configurations;

public class DailyAggregateConfiguration : IEntityTypeConfiguration<DailyAggregate>
{
    public void Configure(EntityTypeBuilder<DailyAggregate> builder)
    {
        // Composite key: one summary per user per day
        builder.HasKey(a => new { a.UserId, a.Date });

        // Decimal precision for monetary values
        builder.Property(a => a.TotalSpent).HasPrecision(18, 2);
    }
}