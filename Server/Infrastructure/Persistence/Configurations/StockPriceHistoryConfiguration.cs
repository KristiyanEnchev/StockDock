namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class StockPriceHistoryConfiguration : IEntityTypeConfiguration<StockPriceHistory>
    {
        public void Configure(EntityTypeBuilder<StockPriceHistory> builder)
        {
            builder.ToTable("stock_price_histories");

            builder.Property(h => h.Price)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(h => h.Timestamp)
                .IsRequired();

            builder.HasIndex(h => new { h.StockId, h.Timestamp });
        }
    }
}
