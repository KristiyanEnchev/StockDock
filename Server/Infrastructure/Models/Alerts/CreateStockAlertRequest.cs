namespace Models.Alerts
{
    public class CreateStockAlertRequest
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal? PriceAbove { get; set; }
        public decimal? PriceBelow { get; set; }
        public decimal? PercentageChange { get; set; }
        public string? NotificationChannel { get; set; }
    }
}