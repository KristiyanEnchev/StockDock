namespace Infrastructure.Services.Stock
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Mapster;

    using Models.Stock;

    using Shared;
    using Shared.Interfaces;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Cache;
    using Application.Interfaces.Watchlist;

    using Domain.Entities.Stock;

    public class WatchlistService : IWatchlistService
    {
        private readonly IRepository<UserWatchlist> _watchlistRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly IStockService _stockService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<WatchlistService> _logger;

        public WatchlistService(
            IRepository<UserWatchlist> watchlistRepository,
            IRepository<Stock> stockRepository,
            IStockService stockService,
            ICacheService cacheService,
            ILogger<WatchlistService> logger)
        {
            _watchlistRepository = watchlistRepository;
            _stockRepository = stockRepository;
            _stockService = stockService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<List<UserWatchlistDto>>> GetUserWatchlistAsync(string userId)
        {
            try
            {
                var cacheKey = $"watchlist_{userId}";
                var cachedWatchlist = await _cacheService.GetAsync<List<UserWatchlistDto>>(cacheKey);

                if (cachedWatchlist != null)
                {
                    return Result<List<UserWatchlistDto>>.SuccessResult(cachedWatchlist);
                }

                var watchlist = await _watchlistRepository.AsNoTracking()
                    .Where(w => w.UserId == userId && w.IsActive)
                    .Include(w => w.Stock)
                    .ToListAsync();

                var symbols = watchlist.Select(w => w.Stock.Symbol).ToList();
                var stocksResult = await _stockService.GetStocksBySymbolsAsync(symbols);

                if (!stocksResult.Success)
                {
                    return Result<List<UserWatchlistDto>>.Failure(stocksResult.Errors);
                }

                var stocksDict = stocksResult.Data.ToDictionary(s => s.Symbol);

                var result = watchlist.Select(w => {
                    var dto = w.Adapt<UserWatchlistDto>();
                    if (stocksDict.TryGetValue(w.Stock.Symbol, out var stock))
                    {
                        dto.Symbol = stock.Symbol;
                        dto.CompanyName = stock.CompanyName;
                        dto.CurrentPrice = stock.CurrentPrice;
                        dto.Change = stock.Change;
                        dto.ChangePercent = stock.ChangePercent;
                    }
                    return dto;
                }).ToList();

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                return Result<List<UserWatchlistDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist for user {UserId}", userId);
                return Result<List<UserWatchlistDto>>.Failure($"Error getting watchlist: {ex.Message}");
            }
        }

        public async Task<Result<UserWatchlistDto>> AddToWatchlistAsync(string userId, string symbol)
        {
            try
            {
                var stock = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == symbol);

                if (stock == null)
                {
                    var stockResult = await _stockService.GetStockBySymbolAsync(symbol);

                    if (!stockResult.Success)
                    {
                        return Result<UserWatchlistDto>.Failure($"Stock not found: {symbol}");
                    }

                    stock = await _stockRepository.AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Symbol == symbol);
                }

                var existingWatchlistItem = await _watchlistRepository.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.Stock.Symbol == symbol);

                if (existingWatchlistItem != null)
                {
                    if (existingWatchlistItem.IsActive)
                    {
                        return Result<UserWatchlistDto>.Failure($"Stock {symbol} is already in your watchlist");
                    }

                    existingWatchlistItem.IsActive = true;
                    await _watchlistRepository.UpdateAsync(existingWatchlistItem);
                    await _watchlistRepository.SaveChangesAsync();

                    stock!.PopularityScore += 1;
                    await _stockRepository.UpdateAsync(stock);
                    await _stockRepository.SaveChangesAsync();

                    await _cacheService.RemoveAsync($"watchlist_{userId}");
                    await _cacheService.RemoveAsync("popular_stocks");

                    return Result<UserWatchlistDto>.SuccessResult(existingWatchlistItem.Adapt<UserWatchlistDto>());
                }

                var watchlistItem = new UserWatchlist
                {
                    UserId = userId,
                    StockId = stock!.Id,
                    IsActive = true
                };

                await _watchlistRepository.AddAsync(watchlistItem);

                stock.PopularityScore += 1;
                await _stockRepository.UpdateAsync(stock);

                await _watchlistRepository.SaveChangesAsync();

                await _cacheService.RemoveAsync($"watchlist_{userId}");
                await _cacheService.RemoveAsync("popular_stocks");

                var result = watchlistItem.Adapt<UserWatchlistDto>();
                result.Symbol = stock.Symbol;
                result.CompanyName = stock.CompanyName;
                result.CurrentPrice = stock.CurrentPrice;

                return Result<UserWatchlistDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock {Symbol} to watchlist for user {UserId}", symbol, userId);
                return Result<UserWatchlistDto>.Failure($"Error adding to watchlist: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveFromWatchlistAsync(string userId, string symbol)
        {
            try
            {
                var watchlistItem = await _watchlistRepository.AsNoTracking()
                    .Include(w => w.Stock)
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.Stock.Symbol == symbol && w.IsActive);

                if (watchlistItem == null)
                {
                    return Result<bool>.Failure($"Stock {symbol} is not in your watchlist");
                }

                watchlistItem.IsActive = false;
                await _watchlistRepository.UpdateAsync(watchlistItem);

                var stock = watchlistItem.Stock;
                stock.PopularityScore = Math.Max(0, stock.PopularityScore - 1);
                await _stockRepository.UpdateAsync(stock);

                await _watchlistRepository.SaveChangesAsync();

                await _cacheService.RemoveAsync($"watchlist_{userId}");
                await _cacheService.RemoveAsync("popular_stocks");

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing stock {Symbol} from watchlist for user {UserId}", symbol, userId);
                return Result<bool>.Failure($"Error removing from watchlist: {ex.Message}");
            }
        }
    }
}