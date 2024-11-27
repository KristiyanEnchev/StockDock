namespace Application.Interfaces
{
    public interface IStockPriceUpdateService
    {
        Task StartMonitoringAsync(CancellationToken cancellationToken);
        Task StopMonitoringAsync();
        Task UpdatePriceAsync(string symbol, decimal newPrice);
        Task<IReadOnlyList<string>> GetActiveSymbolsAsync();
    }
}