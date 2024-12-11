namespace Application.Interfaces.Stock
{
    public interface IStockNotificationService
    {
        Task NotifyPriceChangeAsync(string symbol, decimal oldPrice, decimal newPrice);
        Task NotifyPriceAlertAsync(string userId, string symbol, decimal price, bool isAboveAlert);
        Task NotifyAlertTriggeredAsync(string userId, string symbol, string message, string channel);
    }
}