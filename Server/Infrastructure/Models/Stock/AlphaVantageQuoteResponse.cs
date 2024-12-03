namespace Models.Stock
{
    using System.Text.Json.Serialization;

    public class AlphaVantageQuoteResponse
    {
        [JsonPropertyName("Global Quote")]
        public GlobalQuote? GlobalQuote { get; set; }
    }
}