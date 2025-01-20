namespace Infrastructure.Services.Stock
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Authorization;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Alerts;
    using Application.Interfaces.Identity;
    using Application.Interfaces.Watchlist;

    using Models.Stock;

    using Domain.Entities.Stock;

    public class StockHub : Hub<IStockHub>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly ILogger<StockHub> _logger;
        private readonly IUser _currentUser;
        private readonly IStockAlertService _alertService;

        private static readonly Dictionary<string, HashSet<string>> _userSubscriptions = new();

        public StockHub(
            IWatchlistService watchlistService,
            ILogger<StockHub> logger,
            IUser currentUser,
            IStockAlertService alertService)
        {
            _watchlistService = watchlistService;
            _logger = logger;
            _currentUser = currentUser;
            _alertService = alertService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _currentUser.Id!;
            _logger.LogInformation("User {UserId} connected with connection ID {ConnectionId}",
                userId, Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, "popular_stocks");
            _logger.LogDebug("Added {ConnectionId} to popular_stocks group", Context.ConnectionId);

            var watchlistResult = await _watchlistService.GetUserWatchlistAsync(userId);

            if (watchlistResult.Success)
            {
                foreach (var item in watchlistResult.Data)
                {
                    await SubscribeToStock(item.Symbol);
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _currentUser.Id!;

            if (_userSubscriptions.TryGetValue(userId, out var subscriptions))
            {
                foreach (var symbol in subscriptions)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
                }

                _userSubscriptions.Remove(userId);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "popular_stocks");

            await base.OnDisconnectedAsync(exception);
        }

        [Authorize]
        public async Task SubscribeToStock(string symbol)
        {
            try
            {
                var userId = _currentUser.Id!;

                await Groups.AddToGroupAsync(Context.ConnectionId, symbol);

                if (!_userSubscriptions.TryGetValue(userId, out var subscriptions))
                {
                    subscriptions = new HashSet<string>();
                    _userSubscriptions[userId] = subscriptions;
                }

                subscriptions.Add(symbol);

                _logger.LogInformation("User {UserId} subscribed to stock {Symbol}", userId, symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to stock {Symbol}", symbol);
                throw;
            }
        }

        [Authorize]
        public async Task UnsubscribeFromStock(string symbol)
        {
            try
            {
                var userId = _currentUser.Id!;

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);

                if (_userSubscriptions.TryGetValue(userId, out var subscriptions))
                {
                    subscriptions.Remove(symbol);
                }

                _logger.LogInformation("User {UserId} unsubscribed from stock {Symbol}", userId, symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing from stock {Symbol}", symbol);
                throw;
            }
        }

        [Authorize]
        public async Task AddToWatchlist(string symbol)
        {
            try
            {
                var userId = _currentUser.Id!;

                var result = await _watchlistService.AddToWatchlistAsync(userId, symbol);

                if (result.Success)
                {
                    await SubscribeToStock(symbol);
                    await Clients.Caller.ReceiveStockPriceUpdate(new StockDto
                    {
                        Symbol = result.Data.Symbol,
                        CompanyName = result.Data.CompanyName,
                        CurrentPrice = result.Data.CurrentPrice
                    });
                }
                else
                {
                    _logger.LogWarning("Failed to add {Symbol} to watchlist: {Message}", symbol, result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {Symbol} to watchlist", symbol);
                throw;
            }
        }

        [Authorize]
        public async Task RemoveFromWatchlist(string symbol)
        {
            try
            {
                var userId = _currentUser.Id!;

                var result = await _watchlistService.RemoveFromWatchlistAsync(userId, symbol);

                if (result.Success)
                {
                    await UnsubscribeFromStock(symbol);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing {Symbol} from watchlist", symbol);
                throw;
            }
        }

        [Authorize]
        public async Task CreateAlert(string symbol, AlertType type, decimal threshold)
        {
            try
            {
                var userId = _currentUser.Id!;
                var request = new CreateStockAlertRequest
                {
                    Symbol = symbol,
                    Type = type,
                    Threshold = threshold
                };

                var result = await _alertService.CreateAlertAsync(userId, request);

                if (result.Success)
                {
                    await Clients.Caller.ReceiveAlertCreated(result.Data);
                    _logger.LogInformation("User {UserId} created alert for {Symbol}", userId, symbol);
                }
                else
                {
                    await Clients.Caller.ReceiveError($"Failed to create alert: {result.Errors}");
                    _logger.LogWarning("Failed to create alert for {Symbol}: {Message}", symbol, result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert for {Symbol}", symbol);
                await Clients.Caller.ReceiveError("An error occurred creating the alert.");
            }
        }

        [Authorize]
        public async Task DeleteAlert(string alertId)
        {
            try
            {
                var userId = _currentUser.Id!;
                var result = await _alertService.DeleteAlertAsync(userId, alertId);

                if (result.Success)
                {
                    await Clients.Caller.ReceiveAlertDeleted(alertId);
                    _logger.LogInformation("User {UserId} deleted alert {AlertId}", userId, alertId);
                }
                else
                {
                    await Clients.Caller.ReceiveError($"Failed to delete alert: {result.Errors}");
                    _logger.LogWarning("Failed to delete alert {AlertId}: {Message}", alertId, result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting alert {AlertId}", alertId);
                await Clients.Caller.ReceiveError("An error occurred deleting the alert.");
            }
        }

        [Authorize]
        public async Task GetUserAlerts()
        {
            try
            {
                var userId = _currentUser.Id!;
                var result = await _alertService.GetUserAlertsAsync(userId);

                if (result.Success)
                {
                    await Clients.Caller.ReceiveUserAlerts(result.Data);
                }
                else
                {
                    await Clients.Caller.ReceiveError($"Failed to get alerts: {result.Errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alerts for user {UserId}", _currentUser.Id);
                await Clients.Caller.ReceiveError("An error occurred retrieving alerts.");
            }
        }
    }
}