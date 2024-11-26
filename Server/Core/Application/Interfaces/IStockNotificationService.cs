namespace Application.Interfaces
{
    public interface IStockNotificationService
    {
        Task NotifyPriceChangeAsync(string symbol, decimal oldPrice, decimal newPrice);
        Task NotifyPriceAlertAsync(string userId, string symbol, decimal price, bool isAboveAlert);
    }
}