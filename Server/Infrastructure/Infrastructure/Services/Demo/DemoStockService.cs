namespace Infrastructure.Services.Demo
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Shared.Interfaces;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Cache;

    using Models.Stock;

    using Domain.Entities.Stock;

    using Shared;

    public class DemoStockService : IStockService
    {
        private readonly IRepository<Stock> _stockRepository;
        private readonly IRepository<StockPriceHistory> _historyRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DemoStockService> _logger;

        private const string PopularStocksCacheKey = "demo_popular_stocks";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(5);

        public DemoStockService(
            IRepository<Stock> stockRepository,
            IRepository<StockPriceHistory> historyRepository,
            ICacheService cacheService,
            ILogger<DemoStockService> logger)
        {
            _stockRepository = stockRepository;
            _historyRepository = historyRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<StockDto>> GetStockBySymbolAsync(string symbol)
        {
            try
            {
                var cacheKey = $"demo_stock_{symbol}";
                var cachedStock = await _cacheService.GetAsync<StockDto>(cacheKey);

                if (cachedStock != null)
                {
                    return Result<StockDto>.SuccessResult(cachedStock);
                }

                var stockEntity = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == symbol);

                if (stockEntity == null)
                {
                    return Result<StockDto>.Failure($"Stock {symbol} not found in demo mode.");
                }

                var stockDto = new StockDto
                {
                    Symbol = stockEntity.Symbol,
                    CompanyName = stockEntity.CompanyName,
                    CurrentPrice = stockEntity.CurrentPrice,
                    DayHigh = stockEntity.DayHigh,
                    DayLow = stockEntity.DayLow,
                    OpenPrice = stockEntity.OpenPrice,
                    PreviousClose = stockEntity.PreviousClose,
                    Volume = stockEntity.Volume,
                    LastUpdated = stockEntity.LastUpdated
                };

                await _cacheService.SetAsync(cacheKey, stockDto, CacheExpiry);
                return Result<StockDto>.SuccessResult(stockDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting demo stock {Symbol}", symbol);
                return Result<StockDto>.Failure($"Error retrieving demo stock: {ex.Message}");
            }
        }

        public async Task<Result<List<StockDto>>> GetStocksBySymbolsAsync(List<string> symbols)
        {
            var results = new List<StockDto>();

            foreach (var symbol in symbols)
            {
                var result = await GetStockBySymbolAsync(symbol);
                if (result.Success)
                {
                    results.Add(result.Data);
                }
            }

            return Result<List<StockDto>>.SuccessResult(results);
        }

        public async Task<Result<List<StockDto>>> GetPopularStocksAsync(int limit = 10)
        {
            try
            {
                var cachedStocks = await _cacheService.GetAsync<List<StockDto>>(PopularStocksCacheKey);

                if (cachedStocks != null)
                {
                    return Result<List<StockDto>>.SuccessResult(cachedStocks);
                }

                var stocks = await _stockRepository.AsNoTracking()
                    .OrderByDescending(s => s.PopularityScore)
                    .Take(limit)
                    .ToListAsync();

                var result = stocks.Select(s => new StockDto
                {
                    Symbol = s.Symbol,
                    CompanyName = s.CompanyName,
                    CurrentPrice = s.CurrentPrice,
                    Volume = s.Volume
                }).ToList();

                await _cacheService.SetAsync(PopularStocksCacheKey, result, CacheExpiry);
                return Result<List<StockDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting demo popular stocks");
                return Result<List<StockDto>>.Failure($"Error retrieving demo popular stocks: {ex.Message}");
            }
        }

        public async Task<Result<List<StockPriceHistoryDto>>> GetStockHistoryAsync(string symbol, DateTime from, DateTime to)
        {
            try
            {
                var defaultFrom = DateTime.UtcNow.AddMonths(-6);
                var defaultTo = DateTime.UtcNow;

                if (from < defaultFrom) from = defaultFrom;
                if (to > defaultTo) to = defaultTo;

                var historyCacheKey = $"demo_history_{symbol}_{from:yyyyMMdd}_{to:yyyyMMdd}";
                var cachedHistory = await _cacheService.GetAsync<List<StockPriceHistoryDto>>(historyCacheKey);

                if (cachedHistory != null)
                {
                    return Result<List<StockPriceHistoryDto>>.SuccessResult(cachedHistory);
                }

                var stock = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == symbol);

                if (stock == null)
                {
                    return Result<List<StockPriceHistoryDto>>.Failure($"Stock {symbol} not found.");
                }

                var history = await _historyRepository.AsNoTracking()
                    .Where(h => h.StockId == stock.Id && h.Timestamp >= from && h.Timestamp <= to)
                    .OrderBy(h => h.Timestamp)
                    .Select(h => new StockPriceHistoryDto
                    {
                        Symbol = stock.Symbol,
                        Timestamp = h.Timestamp,
                        Open = h.Open,
                        High = h.High,
                        Low = h.Low,
                        Close = h.Close,
                        Volume = h.Volume
                    })
                    .ToListAsync();

                if (history.Count == 0)
                {
                    return Result<List<StockPriceHistoryDto>>.Failure($"No history found for {symbol}.");
                }

                await _cacheService.SetAsync(historyCacheKey, history, TimeSpan.FromMinutes(5));
                return Result<List<StockPriceHistoryDto>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting demo stock history for {Symbol}", symbol);
                return Result<List<StockPriceHistoryDto>>.Failure($"Error retrieving demo stock history: {ex.Message}");
            }
        }


        public async Task<Result<List<StockDto>>> SearchStocksAsync(string query, string? sortBy = null, bool ascending = true)
        {
            try
            {
                var cacheKey = $"demo_search_{query}_{sortBy}_{ascending}";
                var cachedResults = await _cacheService.GetAsync<List<StockDto>>(cacheKey);

                if (cachedResults != null)
                {
                    return Result<List<StockDto>>.SuccessResult(cachedResults);
                }

                var stocks = await _stockRepository.AsNoTracking()
                    .Where(s => s.Symbol.Contains(query) || s.CompanyName.Contains(query))
                    .ToListAsync();

                if (!string.IsNullOrEmpty(sortBy))
                {
                    stocks = sortBy.ToLower() switch
                    {
                        "symbol" => ascending ? stocks.OrderBy(s => s.Symbol).ToList() : stocks.OrderByDescending(s => s.Symbol).ToList(),
                        "price" => ascending ? stocks.OrderBy(s => s.CurrentPrice).ToList() : stocks.OrderByDescending(s => s.CurrentPrice).ToList(),
                        "volume" => ascending ? stocks.OrderBy(s => s.Volume).ToList() : stocks.OrderByDescending(s => s.Volume).ToList(),
                        _ => stocks
                    };
                }

                var result = stocks.Select(s => new StockDto
                {
                    Symbol = s.Symbol,
                    CompanyName = s.CompanyName,
                    CurrentPrice = s.CurrentPrice,
                    DayHigh = s.DayHigh,
                    DayLow = s.DayLow,
                    OpenPrice = s.OpenPrice,
                    PreviousClose = s.PreviousClose,
                    Volume = s.Volume,
                    LastUpdated = s.LastUpdated
                }).ToList();

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                return Result<List<StockDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stocks in demo mode for query {Query}", query);
                return Result<List<StockDto>>.Failure($"Error searching stocks: {ex.Message}");
            }
        }

    }
}