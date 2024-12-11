namespace Domain.Entities.Stock
{
    using Domain.Events;
    using Domain.Entities.Base;

    public class Stock : BaseAuditableEntity
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal PreviousClose { get; set; }
        public long Volume { get; set; }
        public DateTime LastUpdated { get; set; }
        public int PopularityScore { get; set; } = 0;

        public virtual ICollection<StockPriceHistory> PriceHistory { get; set; } = new List<StockPriceHistory>();
        public virtual ICollection<UserWatchlist> UserWatchlists { get; set; } = new List<UserWatchlist>();
        public virtual ICollection<StockAlert> Alerts { get; set; } = new List<StockAlert>();

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice != CurrentPrice)
            {
                var oldPrice = CurrentPrice;
                CurrentPrice = newPrice;
                LastUpdated = DateTime.UtcNow;
                AddDomainEvent(new StockPriceChangedEvent(Id, oldPrice, newPrice));
            }
        }
    }
}
