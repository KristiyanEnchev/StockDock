namespace Models.Stock
{
    using Domain.Entities.Stock;

    using Models;

    public class StockAlertDto : BaseDto<StockAlertDto, StockAlert>
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public AlertType Type { get; set; }
        public decimal Threshold { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
    }
}