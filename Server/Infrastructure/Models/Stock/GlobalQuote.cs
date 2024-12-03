namespace Models.Stock
{
    using System.Text.Json.Serialization;

    public class GlobalQuote
    {
        [JsonPropertyName("01. symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("02. open")]
        public string Open { get; set; } = "0";

        [JsonPropertyName("03. high")]
        public string High { get; set; } = "0";

        [JsonPropertyName("04. low")]
        public string Low { get; set; } = "0";

        [JsonPropertyName("05. price")]
        public string Price { get; set; } = "0";

        [JsonPropertyName("06. volume")]
        public string Volume { get; set; } = "0";

        [JsonPropertyName("08. previous close")]
        public string PreviousClose { get; set; } = "0";
    }
}