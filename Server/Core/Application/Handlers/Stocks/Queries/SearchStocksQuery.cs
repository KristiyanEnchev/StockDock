namespace Application.Handlers.Stocks.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Stock;

    using Shared;

    using Application.Interfaces.Stock;

    public record SearchStocksQuery(
        string Query,
        string? SortBy = null,
        bool Ascending = true) : IRequest<Result<List<StockDto>>>;

    public class SearchStocksQueryHandler : IRequestHandler<SearchStocksQuery, Result<List<StockDto>>>
    {
        private readonly IStockService _stockService;
        private readonly ILogger<SearchStocksQueryHandler> _logger;

        public SearchStocksQueryHandler(
            IStockService stockService,
            ILogger<SearchStocksQueryHandler> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        public async Task<Result<List<StockDto>>> Handle(SearchStocksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _stockService.SearchStocksAsync(request.Query, request.SortBy, request.Ascending);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stocks with query {Query}", request.Query);
                throw;
            }
        }
    }
}