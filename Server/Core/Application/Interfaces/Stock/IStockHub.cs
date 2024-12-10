namespace Application.Interfaces.Stock
{
    using Models.Stock;

    public interface IStockHub
    {
        Task SubscribeToStock(string symbol);
        Task UnsubscribeFromStock(string symbol);
        Task ReceivePriceUpdate(StockPriceDto priceUpdate);
        Task ReceiveAlert(string message);
    }
}