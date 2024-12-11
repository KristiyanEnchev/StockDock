namespace Application.Interfaces.Stock
{
    using Models.Stock;

    using Shared;

    public interface IExternalStockApi
    {
        Task<Result<StockDto>> GetStockQuoteAsync(string symbol);
        Task<Result<List<StockSearchResultDto>>> SearchStocksAsync(string query);
        Task<Result<List<StockDto>>> GetTopStocksAsync(string category);
        Task<Result<List<StockPriceHistoryDto>>> GetStockHistoryAsync(string symbol, DateTime from, DateTime to);
    }
}