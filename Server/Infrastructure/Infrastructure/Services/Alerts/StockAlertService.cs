namespace Infrastructure.Services.Alerts
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.SignalR;

    using Mapster;

    using Models.Stock;

    using Shared;
    using Shared.Interfaces;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Alerts;

    using Domain.Entities.Stock;

    using Infrastructure.Services.Stock;

    public class StockAlertService : IStockAlertService
    {
        private readonly IRepository<StockAlert> _alertRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly IStockService _stockService;
        private readonly IHubContext<StockHub, IStockHub> _hubContext;
        private readonly ILogger<StockAlertService> _logger;

        public StockAlertService(
            IRepository<StockAlert> alertRepository,
            IRepository<Stock> stockRepository,
            IStockService stockService,
            IHubContext<StockHub, IStockHub> hubContext,
            ILogger<StockAlertService> logger)
        {
            _alertRepository = alertRepository;
            _stockRepository = stockRepository;
            _stockService = stockService;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<Result<List<StockAlertDto>>> GetUserAlertsAsync(string userId)
        {
            try
            {
                var alerts = await _alertRepository.AsNoTracking()
                    .Where(a => a.UserId == userId)
                    .Include(a => a.Stock)
                    .ToListAsync();

                var result = alerts.Select(a =>
                {
                    var dto = a.Adapt<StockAlertDto>();
                    dto.Symbol = a.Stock.Symbol;
                    return dto;
                }).ToList();

                return Result<List<StockAlertDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alerts for user {UserId}", userId);
                return Result<List<StockAlertDto>>.Failure($"Error getting alerts: {ex.Message}");
            }
        }

        public async Task<Result<StockAlertDto>> CreateAlertAsync(string userId, CreateStockAlertRequest request)
        {
            try
            {
                var stock = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == request.Symbol);

                if (stock == null)
                {
                    var stockResult = await _stockService.GetStockBySymbolAsync(request.Symbol);

                    if (!stockResult.Success)
                    {
                        return Result<StockAlertDto>.Failure($"Stock not found: {request.Symbol}");
                    }

                    stock = await _stockRepository.AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Symbol == request.Symbol);
                }

                var existingAlert = await _alertRepository.AsNoTracking()
                    .FirstOrDefaultAsync(a =>
                        a.UserId == userId &&
                        a.StockId == stock!.Id &&
                        a.Type == request.Type &&
                        a.Threshold == request.Threshold);

                if (existingAlert != null)
                {
                    return Result<StockAlertDto>.Failure("A similar alert already exists");
                }

                var alert = new StockAlert
                {
                    UserId = userId,
                    StockId = stock!.Id,
                    Type = request.Type,
                    Threshold = request.Threshold,
                    IsTriggered = false
                };

                await _alertRepository.AddAsync(alert);
                await _alertRepository.SaveChangesAsync();

                var result = alert.Adapt<StockAlertDto>();
                result.Symbol = stock.Symbol;

                return Result<StockAlertDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert for user {UserId}, stock {Symbol}", userId, request.Symbol);
                return Result<StockAlertDto>.Failure($"Error creating alert: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAlertAsync(string userId, string alertId)
        {
            try
            {
                var alert = await _alertRepository.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

                if (alert == null)
                {
                    return Result<bool>.Failure("Alert not found");
                }

                await _alertRepository.DeleteAsync(alert);
                await _alertRepository.SaveChangesAsync();

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting alert {AlertId} for user {UserId}", alertId, userId);
                return Result<bool>.Failure($"Error deleting alert: {ex.Message}");
            }
        }

        public async Task ProcessStockPriceChange(string stockId, decimal oldPrice, decimal newPrice)
        {
            try
            {
                var alerts = await _alertRepository.AsNoTracking()
                    .Where(a => a.StockId == stockId && !a.IsTriggered)
                    .Include(a => a.Stock)
                    .ToListAsync();

                if (!alerts.Any())
                {
                    return;
                }

                foreach (var alert in alerts)
                {
                    bool isTriggered = false;

                    switch (alert.Type)
                    {
                        case AlertType.PriceAbove:
                            isTriggered = newPrice >= alert.Threshold && oldPrice < alert.Threshold;
                            break;
                        case AlertType.PriceBelow:
                            isTriggered = newPrice <= alert.Threshold && oldPrice > alert.Threshold;
                            break;
                        case AlertType.PercentageChange:
                            var percentChange = Math.Abs((newPrice - oldPrice) / oldPrice * 100);
                            isTriggered = percentChange >= alert.Threshold;
                            break;
                    }

                    if (isTriggered)
                    {
                        alert.IsTriggered = true;
                        alert.LastTriggeredAt = DateTime.UtcNow;
                        await _alertRepository.UpdateAsync(alert);

                        var alertDto = alert.Adapt<StockAlertDto>();
                        alertDto.Symbol = alert.Stock.Symbol;

                        await _hubContext.Clients.User(alert.UserId).ReceiveAlertTriggered(alertDto);
                    }
                }

                await _alertRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing price change for stock {StockId}", stockId);
            }
        }
    }
}