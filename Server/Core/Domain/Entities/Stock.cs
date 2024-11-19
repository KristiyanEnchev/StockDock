namespace Domain.Entities
{
    public class Stock : BaseAuditableEntity
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal PreviousClose { get; set; }
        public long Volume { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
