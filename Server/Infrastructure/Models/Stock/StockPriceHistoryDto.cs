namespace Models.Stock
{
    using Domain.Entities;

    public class StockPriceHistoryDto : BaseAuditableDto<StockPriceHistoryDto, StockPriceHistory>
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal? Open { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public decimal? Close { get; set; }
        public long? Volume { get; set; }
    }
}