namespace Models.Stock
{
    using Domain.Entities.Stock;

    using Models;

    public class UserWatchlistDto : BaseDto<UserWatchlistDto, UserWatchlist>
    {
        public string UserId { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
    }
}