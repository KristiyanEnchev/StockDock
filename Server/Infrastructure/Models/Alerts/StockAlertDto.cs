namespace Models.Alerts
{
    public class StockAlertDto : BaseAuditableDto<StockAlertDto, StockAlert>
    {
        public string StockSymbol { get; set; } = string.Empty;
        public decimal? PriceAbove { get; set; }
        public decimal? PriceBelow { get; set; }
        public decimal? PercentageChange { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? LastTriggered { get; set; }
        public string? NotificationChannel { get; set; }
    }
}