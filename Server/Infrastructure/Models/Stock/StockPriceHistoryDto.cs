namespace Models.Stock
{
    using Domain.Entities.Stock;

    using Models;

    public class StockPriceHistoryDto : BaseDto<StockPriceHistoryDto, StockPriceHistory>
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
        public DateTime Timestamp { get; set; }
    }
}