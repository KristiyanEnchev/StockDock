namespace Models.Stock
{
    using Domain.Entities;

    public class UserWatchlistDto : BaseAuditableDto<UserWatchlistDto, UserWatchlist>
    {
        public string UserId { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
        public StockDto Stock { get; set; } = null!;
        public decimal? AlertAbove { get; set; }
        public decimal? AlertBelow { get; set; }
    }
}