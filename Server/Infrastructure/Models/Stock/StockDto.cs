namespace Models.Stock
{
    using Domain.Entities.Stock;

    using Models;

    public class StockDto : BaseDto<StockDto, Stock>
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal PreviousClose { get; set; }
        public long Volume { get; set; }
        public DateTime LastUpdated { get; set; }
        public int PopularityScore { get; set; }
    }
}
