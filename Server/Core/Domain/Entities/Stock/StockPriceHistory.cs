namespace Domain.Entities.Stock
{
    using Domain.Entities.Base;

    public class StockPriceHistory : BaseAuditableEntity
    {
        public string StockId { get; set; } = string.Empty;
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Stock Stock { get; set; } = null!;
    }
}