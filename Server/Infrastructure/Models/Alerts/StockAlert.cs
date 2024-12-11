namespace Models.Alerts
{
    using Domain.Entities;

    public class StockAlert : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string StockId { get; set; } = string.Empty;
        public Stock Stock { get; set; } = null!;
        public decimal? PriceAbove { get; set; }
        public decimal? PriceBelow { get; set; }
        public decimal? PercentageChange { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime? LastTriggered { get; set; }
        public string? NotificationChannel { get; set; } // Email, SMS, Push, etc.
    }
}
