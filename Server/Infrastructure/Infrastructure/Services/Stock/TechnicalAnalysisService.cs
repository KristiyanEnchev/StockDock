namespace Infrastructure.Services.Stock
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Application.Interfaces;

    using Domain.Entities;

    using Models.Stock;

    using Shared.Interfaces;

    using Mapster;

    public class TechnicalAnalysisService : ITechnicalAnalysisService
    {
        private readonly IRepository<StockPriceHistory> _historyRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<TechnicalAnalysisService> _logger;

        public TechnicalAnalysisService(
            IRepository<StockPriceHistory> historyRepository,
            IRepository<Stock> stockRepository,
            ICacheService cacheService,
            ILogger<TechnicalAnalysisService> logger)
        {
            _historyRepository = historyRepository;
            _stockRepository = stockRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TechnicalIndicatorsDto> CalculateIndicatorsAsync(string symbol, string[] indicators)
        {
            try
            {
                var cacheKey = $"technical-indicators:{symbol}";
                var cachedResult = await _cacheService.GetAsync<TechnicalIndicatorsDto>(cacheKey);
                if (cachedResult != null)
                {
                    return cachedResult;
                }

                var result = new TechnicalIndicatorsDto
                {
                    Symbol = symbol,
                    LastUpdated = DateTime.UtcNow
                };

                foreach (var indicator in indicators)
                {
                    switch (indicator.ToLower())
                    {
                        case "ma":
                            result.MovingAverages = new MovingAveragesDto
                            {
                                Sma20 = await CalculateSmaAsync(symbol, 20),
                                Sma50 = await CalculateSmaAsync(symbol, 50),
                                Sma200 = await CalculateSmaAsync(symbol, 200),
                                Ema12 = await CalculateEmaAsync(symbol, 12),
                                Ema26 = await CalculateEmaAsync(symbol, 26)
                            };
                            break;

                        case "rsi":
                            result.Rsi = await CalculateRsiAsync(symbol);
                            break;

                        case "macd":
                            result.Macd = await CalculateMacdAsync(symbol);
                            break;

                        case "bb":
                            result.BollingerBands = await CalculateBollingerBandsAsync(symbol);
                            break;

                        case "volume":
                            result.VolumeIndicators = await CalculateVolumeIndicatorsAsync(symbol);
                            break;
                    }
                }

                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating technical indicators for {Symbol}", symbol);
                throw;
            }
        }

        public async Task<decimal> CalculateSmaAsync(string symbol, int period)
        {
            var prices = await GetHistoricalPricesAsync(symbol, period);
            return prices.Average(p => p.Close ?? p.Price);
        }

        public async Task<decimal> CalculateEmaAsync(string symbol, int period)
        {
            var prices = await GetHistoricalPricesAsync(symbol, period * 2); // Get more data for accuracy
            var closePrices = prices.Select(p => p.Close ?? p.Price).ToList();

            decimal multiplier = 2.0m / (period + 1);
            decimal ema = closePrices[0];

            for (int i = 1; i < closePrices.Count; i++)
            {
                ema = (closePrices[i] * multiplier) + (ema * (1 - multiplier));
            }

            return ema;
        }

        public async Task<RsiDto> CalculateRsiAsync(string symbol, int period = 14)
        {
            var prices = await GetHistoricalPricesAsync(symbol, period + 1);
            var gains = new List<decimal>();
            var losses = new List<decimal>();

            for (int i = 1; i < prices.Count; i++)
            {
                var difference = (prices[i].Close ?? prices[i].Price) -
                               (prices[i - 1].Close ?? prices[i - 1].Price);

                if (difference > 0)
                    gains.Add(difference);
                else
                    losses.Add(Math.Abs(difference));
            }

            var avgGain = gains.Count > 0 ? gains.Average() : 0;
            var avgLoss = losses.Count > 0 ? losses.Average() : 0;

            decimal rs = avgLoss == 0 ? 100m : avgGain / avgLoss;
            decimal rsi = 100m - (100m / (1 + rs));

            return new RsiDto
            {
                Value = rsi,
                Period = period,
                IsOverbought = rsi > 70,
                IsOversold = rsi < 30
            };
        }

        public async Task<MacdDto> CalculateMacdAsync(string symbol)
        {
            var ema12 = await CalculateEmaAsync(symbol, 12);
            var ema26 = await CalculateEmaAsync(symbol, 26);
            var macdLine = ema12 - ema26;

            // Calculate Signal Line (9-day EMA of MACD)
            var prices = await GetHistoricalPricesAsync(symbol, 35);
            var signalLine = await CalculateEmaAsync(symbol, 9);

            return new MacdDto
            {
                MacdLine = macdLine,
                SignalLine = signalLine,
                Histogram = macdLine - signalLine,
                IsBullish = macdLine > signalLine
            };
        }

        public async Task<BollingerBandsDto> CalculateBollingerBandsAsync(
            string symbol,
            int period = 20,
            double standardDeviations = 2)
        {
            var prices = await GetHistoricalPricesAsync(symbol, period);
            var closePrices = prices.Select(p => p.Close ?? p.Price).ToList();

            var sma = closePrices.Average();
            var variance = closePrices.Select(p => Math.Pow((double)(p - sma), 2)).Average();
            var stdDev = (decimal)Math.Sqrt(variance);

            var bandWidth = stdDev * (decimal)standardDeviations;
            var upperBand = sma + bandWidth;
            var lowerBand = sma - bandWidth;
            var currentPrice = closePrices.Last();

            return new BollingerBandsDto
            {
                UpperBand = upperBand,
                MiddleBand = sma,
                LowerBand = lowerBand,
                BandWidth = bandWidth / sma,
                IsPriceAboveUpper = currentPrice > upperBand,
                IsPriceBelowLower = currentPrice < lowerBand
            };
        }

        public async Task<VolumeIndicatorsDto> CalculateVolumeIndicatorsAsync(string symbol)
        {
            var prices = await GetHistoricalPricesAsync(symbol, 20);
            var volumes = prices.Select(p => p.Volume ?? 0L).ToList();

            decimal avgVolume = volumes.Any() ? volumes.Average(v => (decimal)v) : 0m;
            decimal currentVolume = (decimal)(volumes.LastOrDefault());

            decimal obv = 0m;
            for (int i = 1; i < prices.Count; i++)
            {
                var currentClose = prices[i].Close ?? prices[i].Price;
                var previousClose = prices[i - 1].Close ?? prices[i - 1].Price;
                var volume = (decimal)(prices[i].Volume ?? 0L);

                if (currentClose > previousClose)
                    obv += volume;
                else if (currentClose < previousClose)
                    obv -= volume;
            }

            decimal vwap = 0m;
            if (volumes.Any() && volumes.Sum() > 0)
            {
                var priceVolumeSum = prices
                    .Where(p => p.Volume.HasValue)
                    .Sum(p => (p.Close ?? p.Price) * (decimal)(p.Volume ?? 0L));

                var totalVolume = (decimal)volumes.Sum();
                vwap = priceVolumeSum / totalVolume;
            }

            return new VolumeIndicatorsDto
            {
                Obv = obv,
                VolumeRatio = avgVolume > 0 ? currentVolume / avgVolume : 0m,
                Vwap = vwap,
                IsHighVolume = avgVolume > 0 && currentVolume > (avgVolume * 1.5m)
            };
        }

        private async Task<List<StockPriceHistoryDto>> GetHistoricalPricesAsync(string symbol, int days)
        {
            var stock = await _stockRepository.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Symbol == symbol);

            if (stock == null)
                throw new KeyNotFoundException($"Stock {symbol} not found");

            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-days * 2); // Get more data to account for market holidays

            var prices = await _historyRepository
                .AsNoTracking()
                .Where(h => h.StockId == stock.Id &&
                           h.Timestamp >= startDate &&
                           h.Timestamp <= endDate)
                .OrderByDescending(h => h.Timestamp)
                .Take(days)
                .OrderBy(h => h.Timestamp)
                .ProjectToType<StockPriceHistoryDto>()
                .ToListAsync();

            return prices;
        }
    }
}
