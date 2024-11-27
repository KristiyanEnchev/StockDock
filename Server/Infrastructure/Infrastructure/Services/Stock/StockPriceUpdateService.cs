namespace Infrastructure.Services.Stock
{
    using System.Collections.Concurrent;

    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    public class StockPriceUpdateService : IStockPriceUpdateService
    {
        private readonly IStockService _stockService;
        private readonly IStockNotificationService _notificationService;
        private readonly ILogger<StockPriceUpdateService> _logger;
        private readonly Random _random = new();
        private readonly ConcurrentDictionary<string, decimal> _lastPrices = new();
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _monitoringTask;

        public StockPriceUpdateService(
            IStockService stockService,
            IStockNotificationService notificationService,
            ILogger<StockPriceUpdateService> logger)
        {
            _stockService = stockService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task StartMonitoringAsync(CancellationToken cancellationToken)
        {
            if (_monitoringTask != null)
            {
                _logger.LogWarning("Price monitoring is already running");
                return;
            }

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _monitoringTask = MonitorPricesAsync(_cancellationTokenSource.Token);

            await Task.CompletedTask;
        }

        public async Task StopMonitoringAsync()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            if (_monitoringTask != null)
            {
                await _monitoringTask;
                _monitoringTask = null;
            }
        }

        public async Task UpdatePriceAsync(string symbol, decimal newPrice)
        {
            try
            {
                var oldPrice = _lastPrices.GetValueOrDefault(symbol);
                _lastPrices.AddOrUpdate(symbol, newPrice, (_, _) => newPrice);

                var result = await _stockService.UpdateStockPriceAsync(symbol, newPrice);
                if (result.Success)
                {
                    await _notificationService.NotifyPriceChangeAsync(symbol, oldPrice, newPrice);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating price for symbol {Symbol}", symbol);
            }
        }

        public async Task<IReadOnlyList<string>> GetActiveSymbolsAsync()
        {
            var result = await _stockService.GetAllStocksAsync();
            return result.Success
                ? result.Data.Select(s => s.Symbol).ToList()
                : new List<string>();
        }

        private async Task MonitorPricesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var symbols = await GetActiveSymbolsAsync();
                    foreach (var symbol in symbols)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        var currentPrice = _lastPrices.GetValueOrDefault(symbol);
                        if (currentPrice == 0)
                        {
                            var stock = await _stockService.GetStockBySymbolAsync(symbol);
                            if (stock.Success)
                            {
                                currentPrice = stock.Data.CurrentPrice;
                                _lastPrices.TryAdd(symbol, currentPrice);
                            }
                        }

                        var change = (decimal)(_random.NextDouble() * 0.02 - 0.01); // ±1%
                        var newPrice = Math.Max(0.01M, currentPrice * (1 + change));
                        await UpdatePriceAsync(symbol, newPrice);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in price monitoring loop");
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
        }
    }
}