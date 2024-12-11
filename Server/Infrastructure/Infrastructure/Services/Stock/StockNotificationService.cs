namespace Infrastructure.Services.Stock
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;

    using Models.Stock;

    using Application.Interfaces.Stock;

    public class StockNotificationService : IStockNotificationService
    {
        private readonly IHubContext<StockHub, IStockHub> _hubContext;
        private readonly ILogger<StockNotificationService> _logger;

        public StockNotificationService(
            IHubContext<StockHub, IStockHub> hubContext,
            ILogger<StockNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAlertTriggeredAsync(
            string userId,
            string symbol,
            string message,
            string channel)
        {
            try
            {
                switch (channel.ToLower())
                {
                    case "signalr":
                        await _hubContext.Clients.User(userId)
                            .ReceiveAlert($"Alert: {message}");
                        break;

                    default:
                        _logger.LogWarning(
                            "Unsupported notification channel {Channel} for user {UserId}",
                            channel, userId);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending notification to user {UserId} via {Channel}",
                    userId, channel);
                throw;
            }
        }

        public async Task NotifyPriceChangeAsync(string symbol, decimal oldPrice, decimal newPrice)
        {
            try
            {
                var update = new StockPriceDto
                {
                    Symbol = symbol,
                    Price = newPrice,
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group(symbol)
                    .ReceivePriceUpdate(update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error broadcasting price change for {Symbol}",
                    symbol);
                throw;
            }
        }

        public async Task NotifyPriceAlertAsync(
            string userId,
            string symbol,
            decimal price,
            bool isAboveAlert)
        {
            try
            {
                var message = isAboveAlert
                    ? $"{symbol} price is above your alert: {price:C}"
                    : $"{symbol} price is below your alert: {price:C}";

                await _hubContext.Clients.User(userId)
                    .ReceiveAlert(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending price alert to user {UserId} for {Symbol}",
                    userId, symbol);
                throw;
            }
        }
    }
}