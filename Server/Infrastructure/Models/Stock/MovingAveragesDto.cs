namespace Models.Stock
{
    public class MovingAveragesDto
    {
        public decimal Sma20 { get; set; }  // 20-day Simple Moving Average
        public decimal Sma50 { get; set; }  // 50-day Simple Moving Average
        public decimal Sma200 { get; set; } // 200-day Simple Moving Average
        public decimal Ema12 { get; set; }  // 12-day Exponential Moving Average
        public decimal Ema26 { get; set; }  // 26-day Exponential Moving Average
    }
}
