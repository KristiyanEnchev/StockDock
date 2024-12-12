namespace Infrastructure.Services.Stock
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Mapster;

    using Models.Stock;

    using Shared;
    using Shared.Interfaces;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Cache;

    using Domain.Entities.Stock;

    public class StockService : IStockService
    {
        private readonly IRepository<Stock> _stockRepository;
        private readonly IExternalStockApi _externalApi;
        private readonly ICacheService _cacheService;
        private readonly ILogger<StockService> _logger;

        private const string PopularStocksCacheKey = "popular_stocks";
        private const int CacheExpiryMinutes = 5;

        public StockService(
            IRepository<Stock> stockRepository,
            IExternalStockApi externalApi,
            ICacheService cacheService,
            ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _externalApi = externalApi;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<StockDto>> GetStockBySymbolAsync(string symbol)
        {
            try
            {
                var cacheKey = $"stock_{symbol}";
                var cachedStock = await _cacheService.GetAsync<StockDto>(cacheKey);

                if (cachedStock != null)
                {
                    return Result<StockDto>.SuccessResult(cachedStock);
                }

                var stockEntity = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == symbol);

                if (stockEntity != null)
                {
                    if ((DateTime.UtcNow - stockEntity.LastUpdated).TotalMinutes < 5)
                    {
                        var stockDto = stockEntity.Adapt<StockDto>();
                        await _cacheService.SetAsync(cacheKey, stockDto, TimeSpan.FromMinutes(CacheExpiryMinutes));
                        return Result<StockDto>.SuccessResult(stockDto);
                    }
                }

                var apiResult = await _externalApi.GetStockQuoteAsync(symbol);

                if (!apiResult.Success)
                {
                    return Result<StockDto>.Failure(apiResult.Errors);
                }

                var stockData = apiResult.Data;

                if (stockEntity == null)
                {
                    stockEntity = new Stock
                    {
                        Symbol = stockData.Symbol,
                        CompanyName = stockData.CompanyName ?? symbol,
                        CurrentPrice = stockData.CurrentPrice,
                        DayHigh = stockData.DayHigh,
                        DayLow = stockData.DayLow,
                        OpenPrice = stockData.OpenPrice,
                        PreviousClose = stockData.PreviousClose,
                        Volume = stockData.Volume,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _stockRepository.AddAsync(stockEntity);
                }
                else
                {
                    stockEntity.CurrentPrice = stockData.CurrentPrice;
                    stockEntity.DayHigh = stockData.DayHigh;
                    stockEntity.DayLow = stockData.DayLow;
                    stockEntity.OpenPrice = stockData.OpenPrice;
                    stockEntity.PreviousClose = stockData.PreviousClose;
                    stockEntity.Volume = stockData.Volume;
                    stockEntity.LastUpdated = DateTime.UtcNow;

                    await _stockRepository.UpdateAsync(stockEntity);
                }

                await _stockRepository.SaveChangesAsync();

                await _cacheService.SetAsync(cacheKey, stockData, TimeSpan.FromMinutes(CacheExpiryMinutes));

                return Result<StockDto>.SuccessResult(stockData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock for symbol {Symbol}", symbol);
                return Result<StockDto>.Failure($"Error getting stock data: {ex.Message}");
            }
        }

        public async Task<Result<List<StockDto>>> GetStocksBySymbolsAsync(List<string> symbols)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stocks for symbols {Symbols}", string.Join(",", symbols));
                return Result<List<StockDto>>.Failure($"Error getting stocks data: {ex.Message}");
            }
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

                if (stocks.Count < limit)
                {
                    var apiResult = await _externalApi.GetTopStocksAsync("most_active");

                    if (apiResult.Success)
                    {
                        foreach (var stockDto in apiResult.Data)
                        {
                            var existingStock = stocks.FirstOrDefault(s => s.Symbol == stockDto.Symbol);

                            if (existingStock == null)
                            {
                                var newStock = new Stock
                                {
                                    Symbol = stockDto.Symbol,
                                    CompanyName = stockDto.CompanyName ?? stockDto.Symbol,
                                    CurrentPrice = stockDto.CurrentPrice,
                                    DayHigh = stockDto.DayHigh,
                                    DayLow = stockDto.DayLow,
                                    OpenPrice = stockDto.OpenPrice,
                                    PreviousClose = stockDto.PreviousClose,
                                    Volume = stockDto.Volume,
                                    PopularityScore = 50,
                                    LastUpdated = DateTime.UtcNow
                                };

                                await _stockRepository.AddAsync(newStock);
                                stocks.Add(newStock);
                            }
                        }

                        await _stockRepository.SaveChangesAsync();
                    }
                }

                var result = stocks.Adapt<List<StockDto>>();

                await _cacheService.SetAsync(PopularStocksCacheKey, result, TimeSpan.FromMinutes(CacheExpiryMinutes));

                return Result<List<StockDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular stocks");
                return Result<List<StockDto>>.Failure($"Error getting popular stocks: {ex.Message}");
            }
        }

        public async Task<Result<List<StockPriceHistoryDto>>> GetStockHistoryAsync(string symbol, DateTime from, DateTime to)
        {
            try
            {
                var stock = await _stockRepository.AsNoTracking()
                    .Include(s => s.PriceHistory.Where(p => p.Timestamp >= from && p.Timestamp <= to))
                    .FirstOrDefaultAsync(s => s.Symbol == symbol);

                if (stock == null)
                {
                    var stockResult = await GetStockBySymbolAsync(symbol);
                    if (!stockResult.Success)
                    {
                        return Result<List<StockPriceHistoryDto>>.Failure($"Stock not found: {symbol}");
                    }

                    stock = await _stockRepository.AsNoTracking()
                        .Include(s => s.PriceHistory.Where(p => p.Timestamp >= from && p.Timestamp <= to))
                        .FirstOrDefaultAsync(s => s.Symbol == symbol);
                }

                if (stock!.PriceHistory.Count == 0 ||
                    stock.PriceHistory.Min(p => p.Timestamp) > from ||
                    stock.PriceHistory.Max(p => p.Timestamp) < to)
                {
                    var apiResult = await _externalApi.GetStockHistoryAsync(symbol, from, to);

                    if (apiResult.Success)
                    {
                        foreach (var historyDto in apiResult.Data)
                        {
                            if (!stock.PriceHistory.Any(p => p.Timestamp == historyDto.Timestamp))
                            {
                                var history = new StockPriceHistory
                                {
                                    StockId = stock.Id,
                                    Open = historyDto.Open,
                                    High = historyDto.High,
                                    Low = historyDto.Low,
                                    Close = historyDto.Close,
                                    Volume = historyDto.Volume,
                                    Timestamp = historyDto.Timestamp
                                };

                                stock.PriceHistory.Add(history);
                            }
                        }

                        await _stockRepository.UpdateAsync(stock);
                        await _stockRepository.SaveChangesAsync();

                        return Result<List<StockPriceHistoryDto>>.SuccessResult(apiResult.Data);
                    }
                    else
                    {
                        return Result<List<StockPriceHistoryDto>>.Failure(apiResult.Errors);
                    }
                }

                var result = stock.PriceHistory
                    .Where(p => p.Timestamp >= from && p.Timestamp <= to)
                    .OrderBy(p => p.Timestamp)
                    .Adapt<List<StockPriceHistoryDto>>();

                return Result<List<StockPriceHistoryDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock history for symbol {Symbol}", symbol);
                return Result<List<StockPriceHistoryDto>>.Failure($"Error getting stock history: {ex.Message}");
            }
        }

        public async Task<Result<List<StockDto>>> SearchStocksAsync(string query, string? sortBy = null, bool ascending = true)
        {
            try
            {
                var dbStocks = await _stockRepository.AsNoTracking()
                    .Where(s => s.Symbol.Contains(query) || s.CompanyName.Contains(query))
                    .ToListAsync();

                if (dbStocks.Count < 10)
                {
                    var apiResult = await _externalApi.SearchStocksAsync(query);

                    if (apiResult.Success)
                    {
                        foreach (var searchResult in apiResult.Data)
                        {
                            if (!dbStocks.Any(s => s.Symbol == searchResult.Symbol))
                            {
                                var stockResult = await _externalApi.GetStockQuoteAsync(searchResult.Symbol);

                                if (stockResult.Success)
                                {
                                    var newStock = new Stock
                                    {
                                        Symbol = searchResult.Symbol,
                                        CompanyName = searchResult.Name,
                                        CurrentPrice = stockResult.Data.CurrentPrice,
                                        DayHigh = stockResult.Data.DayHigh,
                                        DayLow = stockResult.Data.DayLow,
                                        OpenPrice = stockResult.Data.OpenPrice,
                                        PreviousClose = stockResult.Data.PreviousClose,
                                        Volume = stockResult.Data.Volume,
                                        LastUpdated = DateTime.UtcNow
                                    };

                                    await _stockRepository.AddAsync(newStock);
                                    dbStocks.Add(newStock);
                                }
                            }
                        }

                        await _stockRepository.SaveChangesAsync();
                    }
                }

                var stocks = dbStocks.Adapt<List<StockDto>>();

                if (!string.IsNullOrEmpty(sortBy))
                {
                    stocks = sortBy.ToLower() switch
                    {
                        "symbol" => ascending
                            ? stocks.OrderBy(s => s.Symbol).ToList()
                            : stocks.OrderByDescending(s => s.Symbol).ToList(),
                        "price" => ascending
                            ? stocks.OrderBy(s => s.CurrentPrice).ToList()
                            : stocks.OrderByDescending(s => s.CurrentPrice).ToList(),
                        "volume" => ascending
                            ? stocks.OrderBy(s => s.Volume).ToList()
                            : stocks.OrderByDescending(s => s.Volume).ToList(),
                        "change" => ascending
                            ? stocks.OrderBy(s => s.ChangePercent).ToList()
                            : stocks.OrderByDescending(s => s.ChangePercent).ToList(),
                        _ => stocks
                    };
                }

                return Result<List<StockDto>>.SuccessResult(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stocks for query {Query}", query);
                return Result<List<StockDto>>.Failure($"Error searching stocks: {ex.Message}");
            }
        }
    }
}