namespace Application.Interfaces.Alerts
{
    using Models.Alerts;

    using Shared;

    public interface IStockAlertService
    {
        Task<Result<StockAlertDto>> CreateAlertAsync(string userId, CreateStockAlertRequest request);
        Task<Result<StockAlertDto>> UpdateAlertAsync(string userId, string alertId, UpdateStockAlertRequest request);
        Task<Result<bool>> DeleteAlertAsync(string userId, string alertId);
        Task<Result<IReadOnlyList<StockAlertDto>>> GetUserAlertsAsync(string userId);
        Task ProcessPriceUpdateAsync(string stockId, decimal oldPrice, decimal newPrice);
    }
}