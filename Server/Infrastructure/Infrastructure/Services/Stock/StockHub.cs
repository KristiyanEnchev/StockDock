namespace Infrastructure.Services.Stock
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Identity;
    using Application.Interfaces.Watchlist;

    using Models.Stock;

    [Authorize]
    public class StockHub : Hub<IStockHub>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly IStockService _stockService;
        private readonly ILogger<StockHub> _logger;
        private readonly IUser _currentUser;

        private static readonly Dictionary<string, HashSet<string>> _userSubscriptions = new();

        public StockHub(
            IWatchlistService watchlistService,
            IStockService stockService,
            ILogger<StockHub> logger,
            IUser currentUser)
        {
            _watchlistService = watchlistService;
            _stockService = stockService;
            _logger = logger;
            _currentUser = currentUser;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _currentUser.Id!;

            await Groups.AddToGroupAsync(Context.ConnectionId, "popular_stocks");

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
    }
}