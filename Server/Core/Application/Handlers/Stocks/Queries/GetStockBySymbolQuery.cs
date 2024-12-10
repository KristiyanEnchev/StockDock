namespace Application.Handlers.Stocks.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Stock;

    using Shared;
    using Application.Interfaces.Stock;

    public record GetStockBySymbolQuery(string Symbol) : IRequest<Result<StockDto>>;

    public class GetStockBySymbolQueryHandler : IRequestHandler<GetStockBySymbolQuery, Result<StockDto>>
    {
        private readonly IStockService _stockService;
        private readonly ILogger<GetStockBySymbolQueryHandler> _logger;

        public GetStockBySymbolQueryHandler(
            IStockService stockService,
            ILogger<GetStockBySymbolQueryHandler> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        public async Task<Result<StockDto>> Handle(GetStockBySymbolQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _stockService.GetStockBySymbolAsync(request.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock for symbol {Symbol}", request.Symbol);
                throw;
            }
        }
    }
}