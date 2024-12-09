namespace Models.Stock
{
    public class RsiDto
    {
        public decimal Value { get; set; }   // Current RSI value
        public int Period { get; set; }      // RSI period (typically 14)
        public bool IsOverbought { get; set; }  // RSI > 70
        public bool IsOversold { get; set; }    // RSI < 30
    }
}