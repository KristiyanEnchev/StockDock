namespace Infrastructure.Services.Stock
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Mapster;

    using Domain.Entities;

    using Application.Interfaces;

    using Models.Stock;

    using Shared;
    using Shared.Interfaces;
    using Shared.Exceptions;

    public class WatchlistService : IWatchlistService
    {
        private readonly IRepository<UserWatchlist> _watchlistRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly ILogger<WatchlistService> _logger;

        public WatchlistService(
            IRepository<UserWatchlist> watchlistRepository,
            IRepository<Stock> stockRepository,
            ILogger<WatchlistService> logger)
        {
            _watchlistRepository = watchlistRepository;
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task<Result<UserWatchlistDto>> AddToWatchlistAsync(string userId, CreateWatchlistItemRequest request)
        {
            try
            {
                var stock = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == request.Symbol)
                    ?? throw new CustomException($"Stock with symbol {request.Symbol} not found.");

                var existingWatchlist = await _watchlistRepository.AsNoTracking()
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.StockId == stock.Id);

                if (existingWatchlist != null)
                {
                    return Result<UserWatchlistDto>.Failure($"Stock {request.Symbol} is already in your watchlist.");
                }

                var watchlistItem = new UserWatchlist
                {
                    UserId = userId,
                    StockId = stock.Id,
                    AlertAbove = request.AlertAbove,
                    AlertBelow = request.AlertBelow
                };

                await _watchlistRepository.AddAsync(watchlistItem);
                await _watchlistRepository.SaveChangesAsync();

                var result = await _watchlistRepository
                    .AsNoTracking()
                    .Include(w => w.Stock)
                    .FirstOrDefaultAsync(w => w.Id == watchlistItem.Id);

                return Result<UserWatchlistDto>.SuccessResult(result!.Adapt<UserWatchlistDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock to watchlist for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<bool>> RemoveFromWatchlistAsync(string userId, string symbol)
        {
            try
            {
                var watchlistItem = await _watchlistRepository
                    .AsTracking()
                    .Include(w => w.Stock)
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.Stock.Symbol == symbol);

                if (watchlistItem == null)
                {
                    return Result<bool>.Failure($"Stock {symbol} not found in your watchlist.");
                }

                await _watchlistRepository.DeleteAsync(watchlistItem);
                await _watchlistRepository.SaveChangesAsync();

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing stock from watchlist for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<UserWatchlistDto>>> GetUserWatchlistAsync(string userId)
        {
            try
            {
                var watchlist = await _watchlistRepository
                    .AsNoTracking()
                    .Include(w => w.Stock)
                    .Where(w => w.UserId == userId)
                    .ProjectToType<UserWatchlistDto>()
                    .ToListAsync();

                return Result<IReadOnlyList<UserWatchlistDto>>.SuccessResult(watchlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<bool>> UpdateWatchlistAlertsAsync(string userId, string symbol, decimal? alertAbove, decimal? alertBelow)
        {
            try
            {
                var watchlistItem = await _watchlistRepository
                    .AsTracking()
                    .Include(w => w.Stock)
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.Stock.Symbol == symbol);

                if (watchlistItem == null)
                {
                    return Result<bool>.Failure($"Stock {symbol} not found in your watchlist.");
                }

                if (alertAbove.HasValue && alertBelow.HasValue && alertAbove <= alertBelow)
                {
                    return Result<bool>.Failure("Alert above must be greater than alert below.");
                }

                watchlistItem.AlertAbove = alertAbove;
                watchlistItem.AlertBelow = alertBelow;

                await _watchlistRepository.UpdateAsync(watchlistItem);
                await _watchlistRepository.SaveChangesAsync();

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating watchlist alerts for user {UserId}", userId);
                throw;
            }
        }
    }
}