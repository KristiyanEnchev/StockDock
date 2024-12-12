namespace Models
{
    public class StockApiSettings
    {
        public bool UseDemo { get; set; } = true;
        public string AlphaVantageApiKey { get; set; } = string.Empty;
        public int UpdateIntervalSeconds { get; set; } = 5;
        public int PopularUpdateIntervalSeconds { get; set; } = 15;
    }
}