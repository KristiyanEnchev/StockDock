namespace Models.Stock
{
    public class BollingerBandsDto
    {
        public decimal UpperBand { get; set; }
        public decimal MiddleBand { get; set; }  // 20-day SMA
        public decimal LowerBand { get; set; }
        public decimal BandWidth { get; set; }    // (Upper - Lower) / Middle
        public bool IsPriceAboveUpper { get; set; }
        public bool IsPriceBelowLower { get; set; }
    }
}