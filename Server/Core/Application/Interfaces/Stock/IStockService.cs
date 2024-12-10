namespace Application.Interfaces.Stock
{
    using Models.Stock;

    using Shared;

    public interface IStockService
    {
        Task<Result<StockDto>> GetStockBySymbolAsync(string symbol);
        Task<Result<IReadOnlyList<StockDto>>> GetAllStocksAsync();
        Task<Result<StockDto>> UpdateStockPriceAsync(string symbol, decimal newPrice);
        Task<Result<IReadOnlyList<StockDto>>> GetStocksBySymbolsAsync(IEnumerable<string> symbols);
    }
}