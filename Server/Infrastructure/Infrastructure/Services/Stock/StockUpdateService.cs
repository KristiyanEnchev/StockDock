namespace Infrastructure.Services.Stock
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Mapster;

    using Domain.Entities.Stock;

    using Models.Stock;

    using Shared.Interfaces;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Alerts;

    public class StockUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<StockUpdateService> _logger;
        private readonly Dictionary<string, DateTime> _lastUpdated = new();
        private readonly TimeSpan _popularStocksUpdateInterval = TimeSpan.FromSeconds(15);
        private readonly TimeSpan _watchedStocksUpdateInterval = TimeSpan.FromSeconds(5);

        public StockUpdateService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<StockUpdateService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stock Update Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdatePopularStocks(stoppingToken);
                    await UpdateWatchedStocks(stoppingToken);

                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in stock update service");

                    await Task.Delay(5000, stoppingToken);
                }
            }

            _logger.LogInformation("Stock Update Service is stopping");
        }

        private async Task UpdatePopularStocks(CancellationToken stoppingToken)
        {
            var now = DateTime.UtcNow;

            if (!_lastUpdated.TryGetValue("popular_stocks", out var lastUpdate) ||
                now - lastUpdate >= _popularStocksUpdateInterval)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<StockHub, IStockHub>>();

                var popularStocksResult = await stockService.GetPopularStocksAsync(10);

                if (popularStocksResult.Success)
                {
                    await hubContext.Clients.Group("popular_stocks")
                        .ReceivePopularStocksUpdate(popularStocksResult.Data);

                    _lastUpdated["popular_stocks"] = now;
                }
            }
        }

        private async Task UpdateWatchedStocks(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var stockRepository = scope.ServiceProvider.GetRequiredService<IRepository<Stock>>();
            var externalApi = scope.ServiceProvider.GetRequiredService<IExternalStockApi>();
            var alertService = scope.ServiceProvider.GetRequiredService<IStockAlertService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<StockHub, IStockHub>>();

            var stocksToUpdate = await stockRepository.AsNoTracking()
                .Where(s =>
                    s.PopularityScore > 0 ||
                    s.UserWatchlists.Any(w => w.IsActive))
                .ToListAsync(stoppingToken);

            var now = DateTime.UtcNow;

            foreach (var stock in stocksToUpdate)
            {
                var stockKey = $"stock_{stock.Symbol}";

                if (!_lastUpdated.TryGetValue(stockKey, out var lastUpdate) ||
                    now - lastUpdate >= _watchedStocksUpdateInterval)
                {
                    try
                    {
                        var quoteResult = await externalApi.GetStockQuoteAsync(stock.Symbol);

                        if (quoteResult.Success)
                        {
                            var oldPrice = stock.CurrentPrice;
                            var newPrice = quoteResult.Data.CurrentPrice;

                            if (oldPrice != newPrice)
                            {
                                stock.UpdatePrice(newPrice);
                                stock.DayHigh = quoteResult.Data.DayHigh;
                                stock.DayLow = quoteResult.Data.DayLow;
                                stock.Volume = quoteResult.Data.Volume;
                                await stockRepository.UpdateAsync(stock);
                                await stockRepository.SaveChangesAsync();

                                await alertService.ProcessStockPriceChange(stock.Id, oldPrice, newPrice);

                                var stockDto = stock.Adapt<StockDto>();
                                stockDto.Change = newPrice - stock.PreviousClose;
                                stockDto.ChangePercent = stock.PreviousClose > 0
                                    ? (newPrice - stock.PreviousClose) / stock.PreviousClose * 100
                                    : 0;

                                await hubContext.Clients.Group(stock.Symbol)
                                    .ReceiveStockPriceUpdate(stockDto);
                            }

                            _lastUpdated[stockKey] = now;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating stock {Symbol}", stock.Symbol);
                    }
                }
            }
        }
    }
}