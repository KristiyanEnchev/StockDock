namespace Infrastructure.Services.Alerts
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities;

    using Shared;
    using Shared.Interfaces;

    using Application.Interfaces.Stock;
    using Application.Interfaces.Alerts;

    using Models.Alerts;

    using Mapster;

    public class StockAlertService : IStockAlertService
    {
        private readonly IRepository<StockAlert> _alertRepository;
        private readonly IRepository<Stock> _stockRepository;
        private readonly IStockNotificationService _notificationService;
        private readonly ILogger<StockAlertService> _logger;

        public StockAlertService(
            IRepository<StockAlert> alertRepository,
            IRepository<Stock> stockRepository,
            IStockNotificationService notificationService,
            ILogger<StockAlertService> logger)
        {
            _alertRepository = alertRepository;
            _stockRepository = stockRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<Result<StockAlertDto>> CreateAlertAsync(
            string userId,
            CreateStockAlertRequest request)
        {
            try
            {
                var stock = await _stockRepository.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Symbol == request.Symbol);

                if (stock == null)
                {
                    return Result<StockAlertDto>.Failure($"Stock {request.Symbol} not found.");
                }

                if (!request.PriceAbove.HasValue &&
                    !request.PriceBelow.HasValue &&
                    !request.PercentageChange.HasValue)
                {
                    return Result<StockAlertDto>.Failure(
                        "At least one alert condition must be specified.");
                }

                var alert = new StockAlert
                {
                    UserId = userId,
                    StockId = stock.Id,
                    PriceAbove = request.PriceAbove,
                    PriceBelow = request.PriceBelow,
                    PercentageChange = request.PercentageChange,
                    NotificationChannel = request.NotificationChannel ?? "Email",
                    IsEnabled = true
                };

                await _alertRepository.AddAsync(alert);
                await _alertRepository.SaveChangesAsync();

                alert.Stock = stock; 
                return Result<StockAlertDto>.SuccessResult(alert.Adapt<StockAlertDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<StockAlertDto>> UpdateAlertAsync(
            string userId,
            string alertId,
            UpdateStockAlertRequest request)
        {
            try
            {
                var alert = await _alertRepository.AsTracking()
                    .Include(a => a.Stock)
                    .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

                if (alert == null)
                {
                    return Result<StockAlertDto>.Failure("Alert not found.");
                }

                alert.PriceAbove = request.PriceAbove;
                alert.PriceBelow = request.PriceBelow;
                alert.PercentageChange = request.PercentageChange;
                alert.IsEnabled = request.IsEnabled;
                alert.NotificationChannel = request.NotificationChannel;

                if (!alert.PriceAbove.HasValue &&
                    !alert.PriceBelow.HasValue &&
                    !alert.PercentageChange.HasValue)
                {
                    return Result<StockAlertDto>.Failure(
                        "At least one alert condition must be specified.");
                }

                await _alertRepository.UpdateAsync(alert);
                await _alertRepository.SaveChangesAsync();

                return Result<StockAlertDto>.SuccessResult(alert.Adapt<StockAlertDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating alert {AlertId} for user {UserId}",
                    alertId, userId);
                throw;
            }
        }

        public async Task<Result<bool>> DeleteAlertAsync(string userId, string alertId)
        {
            try
            {
                var alert = await _alertRepository.AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == alertId && a.UserId == userId);

                if (alert == null)
                {
                    return Result<bool>.Failure("Alert not found.");
                }

                await _alertRepository.DeleteAsync(alert);
                await _alertRepository.SaveChangesAsync();

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deleting alert {AlertId} for user {UserId}",
                    alertId, userId);
                throw;
            }
        }

        public async Task<Result<IReadOnlyList<StockAlertDto>>> GetUserAlertsAsync(string userId)
        {
            try
            {
                var alerts = await _alertRepository.AsNoTracking()
                    .Include(a => a.Stock)
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedDate)
                    .ProjectToType<StockAlertDto>()
                    .ToListAsync();

                return Result<IReadOnlyList<StockAlertDto>>.SuccessResult(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alerts for user {UserId}", userId);
                throw;
            }
        }

        public async Task ProcessPriceUpdateAsync(string stockId, decimal oldPrice, decimal newPrice)
        {
            try
            {
                var alerts = await _alertRepository.AsTracking()
                    .Include(a => a.Stock)
                    .Where(a => a.StockId == stockId && a.IsEnabled)
                    .ToListAsync();

                foreach (var alert in alerts)
                {
                    bool shouldTrigger = false;
                    string alertMessage = string.Empty;

                    if (alert.PriceAbove.HasValue &&
                        newPrice > alert.PriceAbove.Value &&
                        oldPrice <= alert.PriceAbove.Value)
                    {
                        shouldTrigger = true;
                        alertMessage = $"{alert.Stock.Symbol} price is now above {alert.PriceAbove:C} (Current: {newPrice:C})";
                    }
                    else if (alert.PriceBelow.HasValue &&
                             newPrice < alert.PriceBelow.Value &&
                             oldPrice >= alert.PriceBelow.Value)
                    {
                        shouldTrigger = true;
                        alertMessage = $"{alert.Stock.Symbol} price is now below {alert.PriceBelow:C} (Current: {newPrice:C})";
                    }
                    else if (alert.PercentageChange.HasValue)
                    {
                        var percentageChange = ((newPrice - oldPrice) / oldPrice) * 100;
                        if (Math.Abs(percentageChange) >= Math.Abs(alert.PercentageChange.Value))
                        {
                            shouldTrigger = true;
                            var direction = percentageChange > 0 ? "up" : "down";
                            alertMessage = $"{alert.Stock.Symbol} has moved {direction} by {Math.Abs(percentageChange):F2}% (Current: {newPrice:C})";
                        }
                    }

                    if (shouldTrigger)
                    {
                        try
                        {
                            await _notificationService.NotifyAlertTriggeredAsync(
                                alert.UserId,
                                alert.Stock.Symbol,
                                alertMessage,
                                alert.NotificationChannel ?? "Email");

                            alert.LastTriggered = DateTime.UtcNow;
                            await _alertRepository.UpdateAsync(alert);
                        }
                        catch (Exception notificationEx)
                        {
                            _logger.LogError(notificationEx,
                                "Failed to send notification for alert {AlertId}", alert.Id);
                        }
                    }
                }

                await _alertRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error processing price update for stock {StockId}. Old price: {OldPrice}, New price: {NewPrice}",
                    stockId, oldPrice, newPrice);
                throw;
            }
        }
    }
}