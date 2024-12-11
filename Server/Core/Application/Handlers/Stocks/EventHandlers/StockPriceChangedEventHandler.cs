namespace Application.Handlers.Stocks.EventHandlers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using MediatR;

    using Domain.Events;

    using Application.Interfaces.Alerts;

    public class StockPriceChangedEventHandler : INotificationHandler<StockPriceChangedEvent>
    {
        private readonly IStockAlertService _alertService;
        private readonly ILogger<StockPriceChangedEventHandler> _logger;

        public StockPriceChangedEventHandler(
            IStockAlertService alertService,
            ILogger<StockPriceChangedEventHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task Handle(StockPriceChangedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Stock {StockId} price changed from {OldPrice} to {NewPrice}",
                notification.StockId,
                notification.OldPrice,
                notification.NewPrice);

            await _alertService.ProcessStockPriceChange(
                notification.StockId,
                notification.OldPrice,
                notification.NewPrice);
        }
    }
}