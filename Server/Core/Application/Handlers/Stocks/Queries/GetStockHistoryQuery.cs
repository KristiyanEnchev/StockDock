namespace Application.Handlers.Stocks.Queries
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using MediatR;

    using Domain.Entities;

    using Models.Stock;
    using Shared;
    using Shared.Interfaces;
    using Mapster;

    public record GetStockHistoryQuery(string Symbol, DateTime From, DateTime To)
        : IRequest<Result<IReadOnlyList<StockPriceHistoryDto>>>;

    public class GetStockHistoryQueryHandler
        : IRequestHandler<GetStockHistoryQuery, Result<IReadOnlyList<StockPriceHistoryDto>>>
    {
        private readonly IRepository<StockPriceHistory> _historyRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly ILogger<GetStockHistoryQueryHandler> _logger;

        public GetStockHistoryQueryHandler(
            IRepository<StockPriceHistory> historyRepository,
            IRepository<Stock> stockRepository,
            ILogger<GetStockHistoryQueryHandler> logger)
        {
            _historyRepository = historyRepository;
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<StockPriceHistoryDto>>> Handle(
            GetStockHistoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var stock = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == request.Symbol);

                if (stock == null)
                {
                    return Result<IReadOnlyList<StockPriceHistoryDto>>
                        .Failure($"Stock with symbol {request.Symbol} not found.");
                }

                var history = await _historyRepository
                    .AsNoTracking()
                    .Where(h => h.StockId == stock.Id
                           && h.Timestamp >= request.From
                           && h.Timestamp <= request.To)
                    .OrderBy(h => h.Timestamp)
                    .ProjectToType<StockPriceHistoryDto>()
                    .ToListAsync(cancellationToken);

                return Result<IReadOnlyList<StockPriceHistoryDto>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting history for stock {Symbol}", request.Symbol);
                throw;
            }
        }
    }
}
