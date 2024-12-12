namespace Infrastructure.Services.Demo
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Options;

    using Application.Interfaces.Stock;

    using Infrastructure.Services.Stock;

    using Models;

    public class DemoStockUpdateService : BackgroundService
    {
        private readonly DemoStockDataProvider _demoProvider;
        private readonly IHubContext<StockHub, IStockHub> _hubContext;
        private readonly ILogger<DemoStockUpdateService> _logger;
        private readonly StockApiSettings _settings;

        public DemoStockUpdateService(
            DemoStockDataProvider demoProvider,
            IHubContext<StockHub, IStockHub> hubContext,
            IOptions<StockApiSettings> settings,
            ILogger<DemoStockUpdateService> logger)
        {
            _demoProvider = demoProvider;
            _hubContext = hubContext;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Demo Stock Update Service started");

            var updateInterval = TimeSpan.FromSeconds(_settings.UpdateIntervalSeconds);
            var popularUpdateInterval = TimeSpan.FromSeconds(_settings.PopularUpdateIntervalSeconds);
            var lastPopularUpdate = DateTime.MinValue;
            var updateCount = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    updateCount++;
                    var startTime = DateTime.UtcNow;
                    _logger.LogInformation("Starting stock update cycle #{Count} at {Time}",
                        updateCount, startTime.ToString("HH:mm:ss.fff"));

                    var symbols = _demoProvider.GetAllSymbols();
                    _logger.LogDebug("Updating {Count} stocks", symbols.Count);

                    int updatedCount = 0;

                    foreach (var symbol in symbols)
                    {
                        _demoProvider.SimulatePriceUpdate(symbol);

                        var stockResult = await _demoProvider.GetStockQuoteAsync(symbol);
                        if (stockResult.Success)
                        {
                            await _hubContext.Clients.Group(symbol)
                                .ReceiveStockPriceUpdate(stockResult.Data);
                            updatedCount++;

                            _logger.LogDebug("Updated {Symbol} price to {Price} ({Change:+0.00;-0.00}%)",
                                symbol,
                                stockResult.Data.CurrentPrice,
                                stockResult.Data.ChangePercent);
                        }
                    }

                    bool updatedPopularStocks = false;
                    if ((DateTime.UtcNow - lastPopularUpdate) >= popularUpdateInterval)
                    {
                        var popularStocksResult = await _demoProvider.GetTopStocksAsync("most_active");
                        if (popularStocksResult.Success)
                        {
                            await _hubContext.Clients.Group("popular_stocks")
                                .ReceivePopularStocksUpdate(popularStocksResult.Data);

                            _logger.LogDebug("Sent popular stocks update. Top stock: {Symbol} at {Price}",
                                popularStocksResult.Data.FirstOrDefault()?.Symbol,
                                popularStocksResult.Data.FirstOrDefault()?.CurrentPrice);

                            lastPopularUpdate = DateTime.UtcNow;
                            updatedPopularStocks = true;
                        }
                    }

                    var endTime = DateTime.UtcNow;
                    var cycleDuration = (endTime - startTime).TotalMilliseconds;

                    _logger.LogInformation("Completed stock update cycle #{Count}. Updated {UpdatedCount} stocks in {Duration:0.00}ms. Popular stocks updated: {PopularUpdated}",
                        updateCount, updatedCount, cycleDuration, updatedPopularStocks);

                    var nextUpdateTime = startTime.Add(updateInterval);
                    var delayTime = nextUpdateTime > endTime
                        ? nextUpdateTime - endTime
                        : TimeSpan.Zero;

                    await Task.Delay(delayTime, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in demo stock update service");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("Demo Stock Update Service is stopping");
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    var updateInterval = TimeSpan.FromSeconds(_settings.UpdateIntervalSeconds);
        //    var popularUpdateInterval = TimeSpan.FromSeconds(_settings.PopularUpdateIntervalSeconds);
        //    var lastPopularUpdate = DateTime.MinValue;

        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            foreach (var symbol in _demoProvider.GetAllSymbols())
        //            {
        //                _demoProvider.SimulatePriceUpdate(symbol);

        //                var stockResult = await _demoProvider.GetStockQuoteAsync(symbol);
        //                if (stockResult.Success)
        //                {
        //                    await _hubContext.Clients.Group(symbol)
        //                        .ReceiveStockPriceUpdate(stockResult.Data);
        //                }
        //            }

        //            if ((DateTime.UtcNow - lastPopularUpdate) >= popularUpdateInterval)
        //            {
        //                var popularStocksResult = await _demoProvider.GetTopStocksAsync("most_active");
        //                if (popularStocksResult.Success)
        //                {
        //                    await _hubContext.Clients.Group("popular_stocks")
        //                        .ReceivePopularStocksUpdate(popularStocksResult.Data);

        //                    lastPopularUpdate = DateTime.UtcNow;
        //                }
        //            }

        //            await Task.Delay(updateInterval, stoppingToken);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error in demo stock update service");
        //            await Task.Delay(1000, stoppingToken);
        //        }
        //    }
        //}
    }
}