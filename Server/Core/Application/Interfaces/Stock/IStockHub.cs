namespace Application.Interfaces.Stock
{
    using Models.Stock;

    public interface IStockHub
    {
        Task ReceiveStockPriceUpdate(StockDto update);
        Task ReceivePopularStocksUpdate(List<StockDto> stocks);
        Task ReceiveAlertTriggered(StockAlertDto alert);

        Task ReceiveAlertCreated(StockAlertDto alert);
        Task ReceiveAlertDeleted(string alertId);
        Task ReceiveUserAlerts(List<StockAlertDto> alerts);
        Task ReceiveError(string message);
    }
}