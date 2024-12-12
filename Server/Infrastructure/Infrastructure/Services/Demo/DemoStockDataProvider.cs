namespace Infrastructure.Services.Demo
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Application.Interfaces.Stock;

    using Models.Stock;

    using Shared;

    public class DemoStockDataProvider : IExternalStockApi
    {
        private static readonly Random _random = new Random();
        private static readonly ConcurrentDictionary<string, DemoStock> _stocks = new();

        private static readonly Dictionary<string, (string Name, decimal BasePrice)> _defaultStocks = new()
        {
            // Technology
            { "AAPL", ("Apple Inc.", 180.50m) },
            { "MSFT", ("Microsoft Corporation", 330.20m) },
            { "GOOGL", ("Alphabet Inc.", 125.80m) },
            { "AMZN", ("Amazon.com Inc.", 130.40m) },
            { "META", ("Meta Platforms Inc.", 290.60m) },
            { "TSLA", ("Tesla Inc.", 240.30m) },
            { "NVDA", ("NVIDIA Corporation", 420.10m) },
            { "INTC", ("Intel Corporation", 42.75m) },
            { "AMD", ("Advanced Micro Devices, Inc.", 105.20m) },
            { "CRM", ("Salesforce, Inc.", 215.80m) },
    
            // Finance
            { "JPM", ("JPMorgan Chase & Co.", 145.70m) },
            { "BAC", ("Bank of America Corporation", 32.50m) },
            { "WFC", ("Wells Fargo & Company", 45.15m) },
            { "GS", ("The Goldman Sachs Group, Inc.", 325.40m) },
            { "V", ("Visa Inc.", 240.90m) },
            { "MA", ("Mastercard Incorporated", 390.30m) },
    
            // Retail
            { "WMT", ("Walmart Inc.", 155.40m) },
            { "TGT", ("Target Corporation", 135.80m) },
            { "COST", ("Costco Wholesale Corporation", 545.20m) },
            { "HD", ("The Home Depot, Inc.", 310.70m) },
    
            // Healthcare
            { "JNJ", ("Johnson & Johnson", 162.30m) },
            { "PFE", ("Pfizer Inc.", 28.40m) },
            { "ABBV", ("AbbVie Inc.", 170.90m) },
            { "MRK", ("Merck & Co., Inc.", 105.60m) },
    
            // Telecommunications
            { "T", ("AT&T Inc.", 16.80m) },
            { "VZ", ("Verizon Communications Inc.", 37.20m) },
    
            // Entertainment
            { "DIS", ("The Walt Disney Company", 89.50m) },
            { "NFLX", ("Netflix, Inc.", 415.70m) },
    
            // Other
            { "KO", ("The Coca-Cola Company", 58.90m) },
            { "PEP", ("PepsiCo, Inc.", 172.60m) }
        };

        public DemoStockDataProvider()
        {
            foreach (var stock in _defaultStocks)
            {
                _stocks.TryAdd(stock.Key, new DemoStock(stock.Key, stock.Value.Name, stock.Value.BasePrice));
            }
        }

        public List<string> GetAllSymbols()
        {
            return _stocks.Keys.ToList();
        }

        public async Task<Result<StockDto>> GetStockQuoteAsync(string symbol)
        {
            try
            {
                if (_stocks.TryGetValue(symbol.ToUpper(), out var stock))
                {
                    return Result<StockDto>.SuccessResult(new StockDto
                    {
                        Symbol = stock.Symbol,
                        CompanyName = stock.Name,
                        CurrentPrice = stock.CurrentPrice,
                        DayHigh = stock.DayHigh,
                        DayLow = stock.DayLow,
                        OpenPrice = stock.OpenPrice,
                        PreviousClose = stock.PreviousClose,
                        Volume = stock.Volume,
                        Change = stock.CurrentPrice - stock.PreviousClose,
                        ChangePercent = ((stock.CurrentPrice - stock.PreviousClose) / stock.PreviousClose) * 100,
                        LastUpdated = DateTime.UtcNow
                    });
                }

                return Result<StockDto>.Failure($"Stock {symbol} not found");
            }
            catch (Exception ex)
            {
                return Result<StockDto>.Failure($"Error getting stock quote: {ex.Message}");
            }
        }

        public async Task<Result<List<StockSearchResultDto>>> SearchStocksAsync(string query)
        {
            try
            {
                query = query.ToUpper();
                var results = _stocks
                    .Where(s => s.Key.Contains(query) || s.Value.Name.ToUpper().Contains(query))
                    .Select(s => new StockSearchResultDto
                    {
                        Symbol = s.Value.Symbol,
                        Name = s.Value.Name,
                        Type = "Equity",
                        Region = "US",
                        Currency = "USD",
                        MatchScore = s.Key.Equals(query, StringComparison.OrdinalIgnoreCase) ? 1.0f : 0.8f
                    })
                    .ToList();

                return Result<List<StockSearchResultDto>>.SuccessResult(results);
            }
            catch (Exception ex)
            {
                return Result<List<StockSearchResultDto>>.Failure($"Error searching stocks: {ex.Message}");
            }
        }

        public async Task<Result<List<StockDto>>> GetTopStocksAsync(string category)
        {
            try
            {
                var stocks = _stocks.Values
                    .OrderByDescending(s => category.ToLower() switch
                    {
                        "gainers" => s.ChangePercent,
                        "losers" => -s.ChangePercent,
                        _ => s.Volume
                    })
                    .Take(10)
                    .Select(s => new StockDto
                    {
                        Symbol = s.Symbol,
                        CompanyName = s.Name,
                        CurrentPrice = s.CurrentPrice,
                        Change = s.CurrentPrice - s.PreviousClose,
                        ChangePercent = s.ChangePercent,
                        Volume = s.Volume,
                        LastUpdated = DateTime.UtcNow
                    })
                    .ToList();

                return Result<List<StockDto>>.SuccessResult(stocks);
            }
            catch (Exception ex)
            {
                return Result<List<StockDto>>.Failure($"Error getting top stocks: {ex.Message}");
            }
        }

        public async Task<Result<List<StockPriceHistoryDto>>> GetStockHistoryAsync(string symbol, DateTime from, DateTime to)
        {
            try
            {
                if (!_stocks.TryGetValue(symbol.ToUpper(), out var stock))
                {
                    return Result<List<StockPriceHistoryDto>>.Failure($"Stock {symbol} not found");
                }

                var history = new List<StockPriceHistoryDto>();
                var currentDate = from.Date;
                var basePrice = stock.BasePrice;
                var trend = _random.NextDouble() * 0.2 - 0.1; // Random trend between -10% and +10%

                while (currentDate <= to.Date)
                {
                    var dailyVolatility = _random.NextDouble() * 0.03; // Up to 3% daily movement
                    var dayChange = basePrice * (decimal)(dailyVolatility + trend);

                    var open = basePrice;
                    var close = basePrice + dayChange;
                    var high = Math.Max(open, close) + basePrice * (decimal)(_random.NextDouble() * 0.01);
                    var low = Math.Min(open, close) - basePrice * (decimal)(_random.NextDouble() * 0.01);
                    var volume = _random.Next(100000, 10000000);

                    history.Add(new StockPriceHistoryDto
                    {
                        Symbol = symbol,
                        Timestamp = currentDate,
                        Open = Math.Round(open, 2),
                        High = Math.Round(high, 2),
                        Low = Math.Round(low, 2),
                        Close = Math.Round(close, 2),
                        Volume = volume
                    });

                    basePrice = close;
                    currentDate = currentDate.AddDays(1);
                }

                return Result<List<StockPriceHistoryDto>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                return Result<List<StockPriceHistoryDto>>.Failure($"Error getting stock history: {ex.Message}");
            }
        }

        public void SimulatePriceUpdate(string symbol)
        {
            if (_stocks.TryGetValue(symbol.ToUpper(), out var stock))
            {
                stock.UpdatePrice();
            }
        }
    }

    internal class DemoStock
    {
        private static readonly Random _random = new Random();
        public decimal BasePrice { get; }
        private readonly decimal _volatility;

        public string Symbol { get; }
        public string Name { get; }
        public decimal CurrentPrice { get; private set; }
        public decimal PreviousClose { get; private set; }
        public decimal OpenPrice { get; private set; }
        public decimal DayHigh { get; private set; }
        public decimal DayLow { get; private set; }
        public long Volume { get; private set; }
        public decimal ChangePercent => ((CurrentPrice - PreviousClose) / PreviousClose) * 100;

        public DemoStock(string symbol, string name, decimal basePrice)
        {
            Symbol = symbol;
            Name = name;
            BasePrice = basePrice;
            _volatility = (decimal)(_random.NextDouble() * 0.02 + 0.01); // 1-3% volatility

            CurrentPrice = basePrice;
            PreviousClose = basePrice;
            OpenPrice = basePrice;
            DayHigh = basePrice;
            DayLow = basePrice;
            Volume = _random.Next(100000, 10000000);
        }

        public void UpdatePrice()
        {
            var changePercent = (decimal)(_random.NextDouble() * 2 - 1) * _volatility;
            var newPrice = Math.Round(CurrentPrice * (1 + changePercent), 2);

            DayHigh = Math.Max(DayHigh, newPrice);
            DayLow = Math.Min(DayLow, newPrice);
            CurrentPrice = newPrice;
            Volume += _random.Next(1000, 100000);
        }
    }
}