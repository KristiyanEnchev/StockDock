namespace Models.Stock
{
    public class TechnicalIndicatorsDto
    {
        public string Symbol { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public MovingAveragesDto? MovingAverages { get; set; }
        public RsiDto? Rsi { get; set; }
        public MacdDto? Macd { get; set; }
        public BollingerBandsDto? BollingerBands { get; set; }
        public VolumeIndicatorsDto? VolumeIndicators { get; set; }
    }
}
