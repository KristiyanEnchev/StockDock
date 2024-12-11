namespace Models.AlphaVantage
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class AlphaVantageTopStocksResponse
    {
        [JsonPropertyName("metadata")]
        public string Metadata { get; set; }

        [JsonPropertyName("last_updated")]
        public string LastUpdated { get; set; }

        [JsonPropertyName("top_gainers")]
        public List<TopStock> TopGainers { get; set; }

        [JsonPropertyName("top_losers")]
        public List<TopStock> TopLosers { get; set; }

        [JsonPropertyName("most_actively_traded")]
        public List<TopStock> MostActivelyTraded { get; set; }
    }

    public class TopStock
    {
        [JsonPropertyName("ticker")]
        public string Symbol { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("change_amount")]
        public string ChangeAmount { get; set; }

        [JsonPropertyName("change_percentage")]
        public string ChangePercentage { get; set; }

        [JsonPropertyName("volume")]
        public string Volume { get; set; }
    }
}
