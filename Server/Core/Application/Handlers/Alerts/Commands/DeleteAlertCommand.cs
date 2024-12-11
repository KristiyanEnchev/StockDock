namespace Application.Handlers.Alerts.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Application.Interfaces.Alerts;

    public record DeleteAlertCommand(string UserId, string AlertId) : IRequest<Result<bool>>;

    public class DeleteAlertCommandHandler : IRequestHandler<DeleteAlertCommand, Result<bool>>
    {
        private readonly IStockAlertService _alertService;
        private readonly ILogger<DeleteAlertCommandHandler> _logger;

        public DeleteAlertCommandHandler(IStockAlertService alertService, ILogger<DeleteAlertCommandHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _alertService.DeleteAlertAsync(request.UserId, request.AlertId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deleting alert {AlertId} for user {UserId}",
                    request.AlertId, request.UserId);
                throw;
            }
        }
    }
}