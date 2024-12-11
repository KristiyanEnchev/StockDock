namespace Application.Handlers.Alerts.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Application.Interfaces.Alerts;

    using Models.Stock;

    public record GetUserAlertsQuery(string UserId) : IRequest<Result<List<StockAlertDto>>>;

    public class GetUserAlertsQueryHandler : IRequestHandler<GetUserAlertsQuery, Result<List<StockAlertDto>>>
    {
        private readonly IStockAlertService _alertService;
        private readonly ILogger<GetUserAlertsQueryHandler> _logger;

        public GetUserAlertsQueryHandler(IStockAlertService alertService, ILogger<GetUserAlertsQueryHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task<Result<List<StockAlertDto>>> Handle(GetUserAlertsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _alertService.GetUserAlertsAsync(request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alerts for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}