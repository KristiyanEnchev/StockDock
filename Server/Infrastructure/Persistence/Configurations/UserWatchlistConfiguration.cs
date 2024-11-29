namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities;

    public class UserWatchlistConfiguration : IEntityTypeConfiguration<UserWatchlist>
    {
        public void Configure(EntityTypeBuilder<UserWatchlist> builder)
        {
            builder.ToTable("user_watchlists");

            builder.Property(w => w.AlertAbove)
                .HasPrecision(18, 6);

            builder.Property(w => w.AlertBelow)
                .HasPrecision(18, 6);

            builder.HasIndex(w => new { w.UserId, w.StockId })
                .IsUnique();

            builder.HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
