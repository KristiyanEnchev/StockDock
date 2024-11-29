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

       
    }
}