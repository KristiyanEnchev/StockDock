namespace Infrastructure.Services.Stock
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Mapster;

    using Domain.Entities;
    using Domain.Events;

    using Models.Stock;

    using Shared;
    using Shared.Interfaces;
    using Shared.Exceptions;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Cache;

    public class StockService : IStockService
    {
        private readonly IRepository<Stock> _stockRepository;
        private readonly ILogger<StockService> _logger;
        private readonly ICacheService _cache;

        public StockService(
            IRepository<Stock> stockRepository,
            ILogger<StockService> logger,
            ICacheService cache)
        {
            _stockRepository = stockRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Result<StockDto>> GetStockBySymbolAsync(string symbol)
        {
            try
            {
                var cached = await _cache.GetAsync<StockDto>($"stock:{symbol}");
                if (cached != null)
                {
                    return Result<StockDto>.SuccessResult(cached);
                }

                var stock = await _stockRepository.FirstOrDefaultAsync<StockDto>(s => s.Symbol == symbol)
                    ?? throw new CustomException($"Stock with symbol {symbol} not found.");

                await _cache.SetAsync($"stock:{symbol}", stock, TimeSpan.FromMinutes(5));

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
                var cached = await _cache.GetAsync<IReadOnlyList<StockDto>>($"stock:all");
                if (cached != null)
                {
                    return Result<IReadOnlyList<StockDto>>.SuccessResult(cached);
                }

                var stocks = await _stockRepository.GetAllAsync<StockDto>();

                await _cache.SetAsync($"stock:all", stocks, TimeSpan.FromMinutes(5));

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
                var cached = await _cache.GetAsync<IReadOnlyList<StockDto>>($"stock:{symbols}");
                if (cached != null)
                {
                    return Result<IReadOnlyList<StockDto>>.SuccessResult(cached);
                }

                var stocks = await _stockRepository
                    .AsNoTracking()
                    .Where(s => symbols.Contains(s.Symbol))
                    .ProjectToType<StockDto>()
                    .ToListAsync();

                await _cache.SetAsync($"stock:{symbols}", stocks, TimeSpan.FromMinutes(5));

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