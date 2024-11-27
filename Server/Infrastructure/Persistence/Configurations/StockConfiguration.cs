namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.ToTable("stocks");

            builder.Property(s => s.Symbol)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.CurrentPrice)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(s => s.DayHigh)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(s => s.DayLow)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(s => s.OpenPrice)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.Property(s => s.PreviousClose)
                .HasPrecision(18, 6)
                .IsRequired();

            builder.HasIndex(s => s.Symbol)
                .IsUnique();

            builder.HasMany<StockPriceHistory>()
                .WithOne(h => h.Stock)
                .HasForeignKey(h => h.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<UserWatchlist>()
                .WithOne(w => w.Stock)
                .HasForeignKey(w => w.StockId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}