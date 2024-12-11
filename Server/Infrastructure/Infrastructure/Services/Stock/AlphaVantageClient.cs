namespace Infrastructure.Services.Stock
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;

    using Application.Interfaces.Stock;

    using Models.Stock;
    using Models.AlphaVantage;

    using Shared;

    public class AlphaVantageClient : IExternalStockApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<AlphaVantageClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public AlphaVantageClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AlphaVantageClient> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AlphaVantage:ApiKey"];
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Result<StockDto>> GetStockQuoteAsync(string symbol)
        {
            try
            {
                var url = $"query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var quoteResponse = JsonSerializer.Deserialize<AlphaVantageStockQuoteResponse>(content, _jsonOptions);

                if (quoteResponse?.GlobalQuote == null)
                {
                    return Result<StockDto>.Failure($"No quote data found for symbol {symbol}");
                }

                var quote = quoteResponse.GlobalQuote;
                var stockDto = new StockDto
                {
                    Symbol = quote.Symbol,
                    CurrentPrice = decimal.Parse(quote.Price),
                    DayHigh = decimal.Parse(quote.High),
                    DayLow = decimal.Parse(quote.Low),
                    OpenPrice = decimal.Parse(quote.Open),
                    PreviousClose = decimal.Parse(quote.PreviousClose),
                    Volume = long.Parse(quote.Volume),
                    LastUpdated = DateTime.Parse(quote.LatestTradingDay)
                };

                return Result<StockDto>.SuccessResult(stockDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock quote for {Symbol}", symbol);
                return Result<StockDto>.Failure($"Error getting stock quote: {ex.Message}");
            }
        }

        public async Task<Result<List<StockSearchResultDto>>> SearchStocksAsync(string query)
        {
            try
            {
                var url = $"query?function=SYMBOL_SEARCH&keywords={query}&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonSerializer.Deserialize<AlphaVantageSearchResponse>(content, _jsonOptions);

                if (searchResponse?.BestMatches == null)
                {
                    return Result<List<StockSearchResultDto>>.SuccessResult(new List<StockSearchResultDto>());
                }

                var results = searchResponse.BestMatches.Select(match => new StockSearchResultDto
                {
                    Symbol = match.Symbol,
                    Name = match.Name,
                    Type = match.Type,
                    Region = match.Region,
                    Currency = match.Currency,
                    MatchScore = float.Parse(match.MatchScore)
                }).ToList();

                return Result<List<StockSearchResultDto>>.SuccessResult(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stocks with query {Query}", query);
                return Result<List<StockSearchResultDto>>.Failure($"Error searching stocks: {ex.Message}");
            }
        }

        public async Task<Result<List<StockDto>>> GetTopStocksAsync(string category)
        {
            try
            {
                var url = $"query?function=TOP_GAINERS_LOSERS&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var topStocksResponse = JsonSerializer.Deserialize<AlphaVantageTopStocksResponse>(content, _jsonOptions);

                if (topStocksResponse == null)
                {
                    return Result<List<StockDto>>.Failure("Failed to get top stocks data");
                }

                List<TopStock> selectedStocks = category.ToLower() switch
                {
                    "gainers" => topStocksResponse.TopGainers,
                    "losers" => topStocksResponse.TopLosers,
                    "most_active" => topStocksResponse.MostActivelyTraded,
                    _ => topStocksResponse.MostActivelyTraded
                };

                var stocks = selectedStocks.Select(stock => new StockDto
                {
                    Symbol = stock.Symbol,
                    CurrentPrice = decimal.Parse(stock.Price),
                    Change = decimal.Parse(stock.ChangeAmount),
                    ChangePercent = decimal.Parse(stock.ChangePercentage.TrimEnd('%')) / 100,
                    Volume = long.Parse(stock.Volume),
                    LastUpdated = DateTime.UtcNow
                }).ToList();

                return Result<List<StockDto>>.SuccessResult(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top stocks for category {Category}", category);
                return Result<List<StockDto>>.Failure($"Error getting top stocks: {ex.Message}");
            }
        }

        public async Task<Result<List<StockPriceHistoryDto>>> GetStockHistoryAsync(string symbol, DateTime from, DateTime to)
        {
            try
            {
                var url = $"query?function=TIME_SERIES_DAILY&symbol={symbol}&outputsize=full&apikey={_apiKey}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var historyResponse = JsonSerializer.Deserialize<AlphaVantageHistoricalDataResponse>(content, _jsonOptions);

                if (historyResponse?.TimeSeries == null)
                {
                    return Result<List<StockPriceHistoryDto>>.Failure($"No historical data found for symbol {symbol}");
                }

                var history = historyResponse.TimeSeries
                    .Where(kvp => DateTime.Parse(kvp.Key) >= from && DateTime.Parse(kvp.Key) <= to)
                    .Select(kvp => new StockPriceHistoryDto
                    {
                        Symbol = symbol,
                        Timestamp = DateTime.Parse(kvp.Key),
                        Open = decimal.Parse(kvp.Value.Open),
                        High = decimal.Parse(kvp.Value.High),
                        Low = decimal.Parse(kvp.Value.Low),
                        Close = decimal.Parse(kvp.Value.Close),
                        Volume = long.Parse(kvp.Value.Volume)
                    })
                    .OrderByDescending(h => h.Timestamp)
                    .ToList();

                return Result<List<StockPriceHistoryDto>>.SuccessResult(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock history for {Symbol}", symbol);
                return Result<List<StockPriceHistoryDto>>.Failure($"Error getting stock history: {ex.Message}");
            }
        }
    }
}