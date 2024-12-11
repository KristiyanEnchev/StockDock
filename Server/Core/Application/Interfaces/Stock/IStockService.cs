namespace Application.Interfaces.Stock
{
    using Models.Stock;

    using Shared;

    public interface IStockService
    {
        Task<Result<StockDto>> GetStockBySymbolAsync(string symbol);
        Task<Result<List<StockDto>>> GetStocksBySymbolsAsync(List<string> symbols);
        Task<Result<List<StockDto>>> GetPopularStocksAsync(int limit = 10);
        Task<Result<List<StockPriceHistoryDto>>> GetStockHistoryAsync(string symbol, DateTime from, DateTime to);
        Task<Result<List<StockDto>>> SearchStocksAsync(string query, string? sortBy = null, bool ascending = true);
    }
}