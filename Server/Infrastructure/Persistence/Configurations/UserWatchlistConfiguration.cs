namespace Persistence.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using Domain.Entities.Stock;

    public class UserWatchlistConfiguration : IEntityTypeConfiguration<UserWatchlist>
    {
        public void Configure(EntityTypeBuilder<UserWatchlist> builder)
        {
            builder.ToTable("user_watchlists");

            builder.HasIndex(w => new { w.UserId, w.StockId })
                .IsUnique();

            builder.HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
