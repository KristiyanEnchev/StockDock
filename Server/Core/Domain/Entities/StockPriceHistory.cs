namespace Domain.Entities
{
    public class StockPriceHistory : BaseAuditableEntity
    {
        public string StockId { get; set; } = string.Empty;
        public Stock Stock { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}