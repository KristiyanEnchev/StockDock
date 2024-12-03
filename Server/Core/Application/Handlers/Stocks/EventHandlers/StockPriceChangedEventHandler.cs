namespace Application.Handlers.Stocks.EventHandlers
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using MediatR;

    using Domain.Events;
    using Domain.Entities;

    using Application.Interfaces;

    using Shared.Interfaces;

    public class StockPriceChangedEventHandler : INotificationHandler<StockPriceChangedEvent>
    {
        private readonly IRepository<UserWatchlist> _watchlistRepository;
        private readonly IStockNotificationService _notificationService;
        private readonly ILogger<StockPriceChangedEventHandler> _logger;

        public StockPriceChangedEventHandler(
            IRepository<UserWatchlist> watchlistRepository,
            IStockNotificationService notificationService,
            ILogger<StockPriceChangedEventHandler> logger)
        {
            _watchlistRepository = watchlistRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(StockPriceChangedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var watchlistItems = await _watchlistRepository
                    .AsNoTracking()
                    .Include(w => w.Stock)
                    .Where(w => w.StockId == notification.StockId)
                    .ToListAsync(cancellationToken);

                foreach (var item in watchlistItems)
                {
                    if (item.AlertAbove.HasValue && notification.NewPrice > item.AlertAbove.Value)
                    {
                        await _notificationService.NotifyPriceAlertAsync(
                            item.UserId,
                            item.Stock.Symbol,
                            notification.NewPrice,
                            true);
                    }
                    else if (item.AlertBelow.HasValue && notification.NewPrice < item.AlertBelow.Value)
                    {
                        await _notificationService.NotifyPriceAlertAsync(
                            item.UserId,
                            item.Stock.Symbol,
                            notification.NewPrice,
                            false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock price change notification for stock {StockId}", notification.StockId);
            }
        }
    }
}