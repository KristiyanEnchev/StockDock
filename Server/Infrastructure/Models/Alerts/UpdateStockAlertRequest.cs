namespace Models.Alerts
{
    public class UpdateStockAlertRequest
    {
        public decimal? PriceAbove { get; set; }
        public decimal? PriceBelow { get; set; }
        public decimal? PercentageChange { get; set; }
        public bool IsEnabled { get; set; }
        public string? NotificationChannel { get; set; }
    }
}