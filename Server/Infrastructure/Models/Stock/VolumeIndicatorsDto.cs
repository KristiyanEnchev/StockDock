namespace Models.Stock
{
    public class VolumeIndicatorsDto
    {
        public decimal Obv { get; set; }          // On-Balance Volume
        public decimal VolumeRatio { get; set; }  // Current/Average Volume Ratio
        public decimal Vwap { get; set; }         // Volume Weighted Average Price
        public bool IsHighVolume { get; set; }    // Volume > Average * 1.5
    }
}