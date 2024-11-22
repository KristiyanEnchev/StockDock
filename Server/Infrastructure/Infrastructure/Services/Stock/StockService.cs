namespace Infrastructure.Services.Stock
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Mapster;

    using Domain.Entities;
    using Domain.Events;

    using Application.Interfaces;

    using Models.Stock;

    using Shared;
    using Shared.Interfaces;
    using Shared.Exceptions;

    public class StockService 
    {
        private readonly IRepository<Stock> _stockRepository;
        private readonly ILogger<StockService> _logger;

        public StockService(
            IRepository<Stock> stockRepository,
            ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task<Result<StockDto>> GetStockBySymbolAsync(string symbol)
        {
            try
            {
                var stock = await _stockRepository.FirstOrDefaultAsync<StockDto>(s => s.Symbol == symbol)
                    ?? throw new CustomException($"Stock with symbol {symbol} not found.");

                return Result<StockDto>.SuccessResult(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock for symbol {Symbol}", symbol);
                throw;
            }
        }

        
    }
}