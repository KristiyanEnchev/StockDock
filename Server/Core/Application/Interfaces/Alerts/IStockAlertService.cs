namespace Application.Interfaces.Alerts
{
    using Models.Stock;

    using Shared;

    public interface IStockAlertService
    {
        Task<Result<List<StockAlertDto>>> GetUserAlertsAsync(string userId);
        Task<Result<StockAlertDto>> CreateAlertAsync(string userId, CreateStockAlertRequest request);
        Task<Result<bool>> DeleteAlertAsync(string userId, string alertId);
        Task ProcessStockPriceChange(string stockId, decimal oldPrice, decimal newPrice);
    }
}