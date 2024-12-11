namespace Application.Handlers.Alerts.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Models.Alerts;

    using Application.Interfaces.Alerts;

    public record UpdateAlertCommand(
        string UserId,
        string AlertId,
        UpdateStockAlertRequest Request) : IRequest<Result<StockAlertDto>>;

    public class UpdateAlertCommandHandler
        : IRequestHandler<UpdateAlertCommand, Result<StockAlertDto>>
    {
        private readonly IStockAlertService _alertService;
        private readonly ILogger<UpdateAlertCommandHandler> _logger;

        public UpdateAlertCommandHandler(
            IStockAlertService alertService,
            ILogger<UpdateAlertCommandHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task<Result<StockAlertDto>> Handle(
            UpdateAlertCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _alertService.UpdateAlertAsync(
                    request.UserId,
                    request.AlertId,
                    request.Request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating alert {AlertId} for user {UserId}",
                    request.AlertId, request.UserId);
                throw;
            }
        }
    }
}