namespace Infrastructure.Services.Stock
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Authorization;

    using Application.Interfaces.Stock;

    [Authorize]
    public class StockHub : Hub<IStockHub>
    {
        private readonly ILogger<StockHub> _logger;

        public StockHub(ILogger<StockHub> logger)
        {
            _logger = logger;
        }

        public async Task SubscribeToStock(string symbol)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
                _logger.LogInformation("Client {ConnectionId} subscribed to {Symbol}", Context.ConnectionId, symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to stock {Symbol}", symbol);
                throw;
            }
        }

        public async Task UnsubscribeFromStock(string symbol)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
                _logger.LogInformation("Client {ConnectionId} unsubscribed from {Symbol}", Context.ConnectionId, symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from stock {Symbol}", symbol);
                throw;
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client {ConnectionId} connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}