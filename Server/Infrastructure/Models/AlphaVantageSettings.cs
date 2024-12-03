namespace Models
{
    public class AlphaVantageSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://www.alphavantage.co/query";
        public int RequestsPerMinute { get; set; } = 5;
    }
}