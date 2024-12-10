namespace Infrastructure.Services.Stock
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities;

    using Shared;
    using Shared.Interfaces;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Subscription;

    public class StockSubscriptionService : IStockSubscriptionService
    {
        private readonly IRepository<UserStockSubscription> _subscriptionRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly IStockHub _stockHub;
        private readonly ILogger<StockSubscriptionService> _logger;

        public StockSubscriptionService(
            IRepository<UserStockSubscription> subscriptionRepository,
            IRepository<Stock> stockRepository,
            IStockHub stockHub,
            ILogger<StockSubscriptionService> logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _stockRepository = stockRepository;
            _stockHub = stockHub;
            _logger = logger;
        }

        public async Task<Result<bool>> SubscribeAsync(string userId, string symbol)
        {
            try
            {
                var stock = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == symbol);

                if (stock == null)
                {
                    return Result<bool>.Failure($"Stock with symbol {symbol} not found.");
                }

                var existingSubscription = await _subscriptionRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.StockId == stock.Id);

                if (existingSubscription != null)
                {
                    return Result<bool>.Failure($"Already subscribed to {symbol}.");
                }

                var subscription = new UserStockSubscription
                {
                    UserId = userId,
                    StockId = stock.Id,
                    SubscribedAt = DateTime.UtcNow
                };

                await _subscriptionRepository.AddAsync(subscription);
                await _subscriptionRepository.SaveChangesAsync();

                await _stockHub.SubscribeToStock(symbol);

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing user {UserId} to stock {Symbol}", userId, symbol);
                throw;
            }
        }

        public async Task<Result<bool>> UnsubscribeAsync(string userId, string symbol)
        {
            try
            {
                var subscription = await _subscriptionRepository
                    .AsTracking()
                    .Include(s => s.Stock)
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.Stock.Symbol == symbol);

                if (subscription == null)
                {
                    return Result<bool>.Failure($"No active subscription found for {symbol}.");
                }

                await _subscriptionRepository.DeleteAsync(subscription);
                await _subscriptionRepository.SaveChangesAsync();

                await _stockHub.UnsubscribeFromStock(symbol);

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing user {UserId} from stock {Symbol}", userId, symbol);
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<string>>> GetActiveSubscriptionsAsync(string userId)
        {
            try
            {
                var subscriptions = await _subscriptionRepository
                    .AsNoTracking()
                    .Include(s => s.Stock)
                    .Where(s => s.UserId == userId)
                    .Select(s => s.Stock.Symbol)
                    .ToListAsync();

                return Result<IReadOnlyList<string>>.SuccessResult(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscriptions for user {UserId}", userId);
                throw;
            }
        }
    }

}
