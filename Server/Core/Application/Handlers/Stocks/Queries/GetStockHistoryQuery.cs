namespace Application.Handlers.Stocks.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Stock;

    using Shared;

    using Application.Interfaces.Stock;

    public record GetStockHistoryQuery(
        string Symbol,
        DateTime From,
        DateTime To) : IRequest<Result<List<StockPriceHistoryDto>>>;

    public class GetStockHistoryQueryHandler : IRequestHandler<GetStockHistoryQuery, Result<List<StockPriceHistoryDto>>>
    {
        private readonly IStockService _stockService;
        private readonly ILogger<GetStockHistoryQueryHandler> _logger;

        public GetStockHistoryQueryHandler(
            IStockService stockService,
            ILogger<GetStockHistoryQueryHandler> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        public async Task<Result<List<StockPriceHistoryDto>>> Handle(GetStockHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _stockService.GetStockHistoryAsync(request.Symbol, request.From, request.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock history for symbol {Symbol}", request.Symbol);
                throw;
            }
        }
    }
}