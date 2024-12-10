namespace Application.Interfaces.Stock
{
    using Models.Stock;

    public interface ITechnicalAnalysisService
    {
        Task<TechnicalIndicatorsDto> CalculateIndicatorsAsync(string symbol, string[] indicators);
        Task<decimal> CalculateSmaAsync(string symbol, int period);
        Task<decimal> CalculateEmaAsync(string symbol, int period);
        Task<RsiDto> CalculateRsiAsync(string symbol, int period = 14);
        Task<MacdDto> CalculateMacdAsync(string symbol);
        Task<BollingerBandsDto> CalculateBollingerBandsAsync(string symbol, int period = 20, double standardDeviations = 2);
        Task<VolumeIndicatorsDto> CalculateVolumeIndicatorsAsync(string symbol);
    }
}