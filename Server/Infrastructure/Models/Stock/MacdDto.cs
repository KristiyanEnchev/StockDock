namespace Models.Stock
{
    public class MacdDto
    {
        public decimal MacdLine { get; set; }     // MACD Line
        public decimal SignalLine { get; set; }   // Signal Line
        public decimal Histogram { get; set; }    // MACD Histogram
        public bool IsBullish { get; set; }       // MACD > Signal
    }
}
