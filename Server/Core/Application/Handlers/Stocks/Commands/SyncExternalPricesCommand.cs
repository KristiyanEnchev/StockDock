namespace Application.Handlers.Stocks.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;
    using Application.Interfaces.Stock;

    public record SyncExternalPricesCommand : IRequest<Result<int>>;

    public class SyncExternalPricesCommandHandler
        : IRequestHandler<SyncExternalPricesCommand, Result<int>>
    {
        private readonly IExternalStockApi _externalApi;
        private readonly IStockService _stockService;
        private readonly ILogger<SyncExternalPricesCommandHandler> _logger;

        public SyncExternalPricesCommandHandler(
            IExternalStockApi externalApi,
            IStockService stockService,
            ILogger<SyncExternalPricesCommandHandler> logger)
        {
            _externalApi = externalApi;
            _stockService = stockService;
            _logger = logger;
        }

        public async Task<Result<int>> Handle(
            SyncExternalPricesCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var stocks = await _stockService.GetAllStocksAsync();
                if (!stocks.Success)
                {
                    return Result<int>.Failure(stocks.Errors);
                }

                var updateCount = 0;
                foreach (var stock in stocks.Data)
                {
                    try
                    {
                        var externalData = await _externalApi.GetStockQuoteAsync(stock.Symbol);
                        if (externalData.Success)
                        {
                            await _stockService.UpdateStockPriceAsync(
                                stock.Symbol,
                                externalData.Data.CurrentPrice);
                            updateCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to sync price for {Symbol}", stock.Symbol);
                    }

                    await Task.Delay(200, cancellationToken);
                }

                return Result<int>.SuccessResult(updateCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing external prices");
                throw;
            }
        }
    }
}