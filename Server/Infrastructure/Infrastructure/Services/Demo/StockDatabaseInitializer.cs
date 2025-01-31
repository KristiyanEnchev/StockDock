namespace Infrastructure.Services.Demo
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities.Stock;

    using Shared.Interfaces;

    public class StockDatabaseInitializer
    {
        private readonly DemoStockDataProvider _demoProvider;
        private readonly IRepository<Stock> _stockRepository;
        private readonly IRepository<StockPriceHistory> _historyRepository;
        private readonly ILogger<StockDatabaseInitializer> _logger;

        public StockDatabaseInitializer(
            DemoStockDataProvider demoProvider,
            IRepository<Stock> stockRepository,
            IRepository<StockPriceHistory> historyRepository,
            ILogger<StockDatabaseInitializer> logger)
        {
            _demoProvider = demoProvider;
            _stockRepository = stockRepository;
            _historyRepository = historyRepository;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing stock database with demo data...");

            var symbols = _demoProvider.GetAllSymbols();
            var startDate = DateTime.UtcNow.AddMonths(-6);

            foreach (var symbol in symbols)
            {
                var stockResult = await _demoProvider.GetStockQuoteAsync(symbol);
                if (!stockResult.Success) continue;

                var existingStock = await _stockRepository.AsNoTracking().FirstOrDefaultAsync(s => s.Symbol == symbol);
                if (existingStock == null)
                {
                    var newStock = new Stock
                    {
                        Symbol = stockResult.Data.Symbol,
                        CompanyName = stockResult.Data.CompanyName,
                        CurrentPrice = stockResult.Data.CurrentPrice,
                        DayHigh = stockResult.Data.DayHigh,
                        DayLow = stockResult.Data.DayLow,
                        OpenPrice = stockResult.Data.OpenPrice,
                        PreviousClose = stockResult.Data.PreviousClose,
                        Volume = stockResult.Data.Volume,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _stockRepository.AddAsync(newStock);
                    await _stockRepository.SaveChangesAsync();
                    _logger.LogInformation("Added stock {Symbol} to database.", symbol);
                    existingStock = newStock;
                }

                var historyResult = await _demoProvider.GetStockHistoryAsync(symbol, startDate, DateTime.UtcNow);
                if (historyResult.Success)
                {
                    foreach (var history in historyResult.Data)
                    {
                        if (!await _historyRepository.AsNoTracking()
                            .AnyAsync(h => h.StockId == existingStock.Id && h.Timestamp.Date == history.Timestamp.Date))
                        {
                            await _historyRepository.AddAsync(new StockPriceHistory
                            {
                                StockId = existingStock.Id,
                                Open = history.Open,
                                High = history.High,
                                Low = history.Low,
                                Close = history.Close,
                                Volume = history.Volume,
                                Timestamp = history.Timestamp
                            });
                        }
                    }

                    await _historyRepository.SaveChangesAsync();
                    _logger.LogInformation("Generated historical data for {Symbol}.", symbol);
                }
            }

            _logger.LogInformation("Stock database initialization complete.");
        }
    }

}
