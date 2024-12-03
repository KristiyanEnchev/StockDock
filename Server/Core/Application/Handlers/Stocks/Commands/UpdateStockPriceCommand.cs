namespace Application.Handlers.Stocks.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Application.Interfaces;

    using Models.Stock;

    using Shared;

    public record UpdateStockPriceCommand(string Symbol, decimal NewPrice) : IRequest<Result<StockDto>>;

    public class UpdateStockPriceCommandHandler : IRequestHandler<UpdateStockPriceCommand, Result<StockDto>>
    {
        private readonly IStockService _stockService;
        private readonly ILogger<UpdateStockPriceCommandHandler> _logger;

        public UpdateStockPriceCommandHandler(
            IStockService stockService,
            ILogger<UpdateStockPriceCommandHandler> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        public async Task<Result<StockDto>> Handle(UpdateStockPriceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _stockService.UpdateStockPriceAsync(request.Symbol, request.NewPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock price for {Symbol}", request.Symbol);
                throw;
            }
        }
    }
}