namespace Domain.Events
{
    public class StockPriceChangedEvent : BaseEvent
    {
        public string StockId { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public DateTime Timestamp { get; }

        public StockPriceChangedEvent(string stockId, decimal oldPrice, decimal newPrice)
        {
            StockId = stockId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            Timestamp = DateTime.UtcNow;
        }
    }
}
