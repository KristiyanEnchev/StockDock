namespace Application.Interfaces.Stock
{
    using Models.Stock;

    public interface IStockHub
    {
        Task ReceiveStockPriceUpdate(StockDto update);
        Task ReceiveAlertTriggered(StockAlertDto alert);
        Task ReceivePopularStocksUpdate(List<StockDto> stocks);
    }
}