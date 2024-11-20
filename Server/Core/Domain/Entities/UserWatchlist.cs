namespace Domain.Entities
{
    public class UserWatchlist : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string StockId { get; set; } = string.Empty;
        public Stock Stock { get; set; } = null!;
        public decimal? AlertAbove { get; set; }
        public decimal? AlertBelow { get; set; }
    }
}