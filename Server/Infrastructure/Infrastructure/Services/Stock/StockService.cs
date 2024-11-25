namespace Infrastructure.Services.Stock
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Mapster;

    using Domain.Entities;
    using Domain.Events;

    using Application.Interfaces;

    using Models.Stock;

    using Shared;
    using Shared.Interfaces;
    using Shared.Exceptions;

    public class StockService : IStockService
    {
        private readonly IRepository<Stock> _stockRepository;
        private readonly ILogger<StockService> _logger;

        public StockService(
            IRepository<Stock> stockRepository,
            ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task<Result<StockDto>> GetStockBySymbolAsync(string symbol)
        {
            try
            {
                var stock = await _stockRepository.FirstOrDefaultAsync<StockDto>(s => s.Symbol == symbol)
                    ?? throw new CustomException($"Stock with symbol {symbol} not found.");

                return Result<StockDto>.SuccessResult(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock for symbol {Symbol}", symbol);
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<StockDto>>> GetAllStocksAsync()
        {
            try
            {
                var stocks = await _stockRepository.GetAllAsync<StockDto>();
                return Result<IReadOnlyList<StockDto>>.SuccessResult(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stocks");
                throw;
            }
        }

        public async Task<Result<StockDto>> UpdateStockPriceAsync(string symbol, decimal newPrice)
        {
            try
            {
                var stock = await _stockRepository.AsTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == symbol)
                    ?? throw new CustomException($"Stock with symbol {symbol} not found.");

                var oldPrice = stock.CurrentPrice;
                stock.CurrentPrice = newPrice;
                stock.LastUpdated = DateTime.UtcNow;

                if (newPrice > stock.DayHigh) stock.DayHigh = newPrice;
                if (newPrice < stock.DayLow) stock.DayLow = newPrice;

                stock.AddDomainEvent(new StockPriceChangedEvent(stock.Id, oldPrice, newPrice));

                await _stockRepository.UpdateAsync(stock);
                await _stockRepository.SaveChangesAsync();

                return Result<StockDto>.SuccessResult(stock.Adapt<StockDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock price for symbol {Symbol}", symbol);
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<StockDto>>> GetStocksBySymbolsAsync(IEnumerable<string> symbols)
        {
            try
            {
                var stocks = await _stockRepository
                    .AsNoTracking()
                    .Where(s => symbols.Contains(s.Symbol))
                    .ProjectToType<StockDto>()
                    .ToListAsync();

                return Result<IReadOnlyList<StockDto>>.SuccessResult(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stocks for symbols {Symbols}", string.Join(", ", symbols));
                throw;
            }
        }
    }
}