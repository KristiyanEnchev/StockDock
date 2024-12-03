namespace Application.Interfaces
{
    using Models.Stock;

    using Shared;

    public interface IExternalStockApi
    {
        Task<Result<StockDto>> GetStockQuoteAsync(string symbol);
    }
}