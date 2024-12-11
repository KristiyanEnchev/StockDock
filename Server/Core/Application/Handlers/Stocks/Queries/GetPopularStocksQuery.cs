namespace Application.Handlers.Stocks.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Stock;

    using Shared;

    using Application.Interfaces.Stock;

    public record GetPopularStocksQuery(int Limit = 10) : IRequest<Result<List<StockDto>>>;

    public class GetPopularStocksQueryHandler : IRequestHandler<GetPopularStocksQuery, Result<List<StockDto>>>
    {
        private readonly IStockService _stockService;
        private readonly ILogger<GetPopularStocksQueryHandler> _logger;

        public GetPopularStocksQueryHandler(
            IStockService stockService,
            ILogger<GetPopularStocksQueryHandler> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        public async Task<Result<List<StockDto>>> Handle(GetPopularStocksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _stockService.GetPopularStocksAsync(request.Limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular stocks");
                throw;
            }
        }
    }
}