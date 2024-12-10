namespace Infrastructure.Services.Stock
{
    using System.Net.Http;
    using System.Text.Json;

    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Logging;

    using Models;
    using Models.Stock;

    using Shared;

    using Application.Interfaces.Stock;

    public class AlphaVantageClient : IExternalStockApi
    {
        private readonly HttpClient _httpClient;
        private readonly AlphaVantageSettings _settings;
        private readonly ILogger<AlphaVantageClient> _logger;
        private readonly SemaphoreSlim _rateLimiter;

        public AlphaVantageClient(
            HttpClient httpClient,
            IOptions<AlphaVantageSettings> settings,
            ILogger<AlphaVantageClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
            _rateLimiter = new SemaphoreSlim(_settings.RequestsPerMinute, _settings.RequestsPerMinute);
        }

        public async Task<Result<StockDto>> GetStockQuoteAsync(string symbol)
        {
            try
            {
                await _rateLimiter.WaitAsync();

                var url = $"{_settings.BaseUrl}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_settings.ApiKey}";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<AlphaVantageQuoteResponse>(content);

                if (data?.GlobalQuote == null)
                {
                    return Result<StockDto>.Failure("No data returned from Alpha Vantage");
                }

                var stock = new StockDto
                {
                    Symbol = symbol,
                    CurrentPrice = decimal.Parse(data.GlobalQuote.Price),
                    OpenPrice = decimal.Parse(data.GlobalQuote.Open),
                    DayHigh = decimal.Parse(data.GlobalQuote.High),
                    DayLow = decimal.Parse(data.GlobalQuote.Low),
                    PreviousClose = decimal.Parse(data.GlobalQuote.PreviousClose),
                    Volume = long.Parse(data.GlobalQuote.Volume),
                    LastUpdated = DateTime.UtcNow
                };

                return Result<StockDto>.SuccessResult(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stock data from Alpha Vantage for {Symbol}", symbol);
                return Result<StockDto>.Failure($"Error fetching stock data: {ex.Message}");
            }
            finally
            {
                _ = Task.Delay(TimeSpan.FromSeconds(60))
                    .ContinueWith(_ => _rateLimiter.Release());
            }
        }
    }
}
