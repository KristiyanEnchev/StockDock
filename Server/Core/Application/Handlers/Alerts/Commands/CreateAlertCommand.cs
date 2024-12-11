namespace Application.Handlers.Alerts.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Models.Alerts;

    using Application.Interfaces.Alerts;

    public record CreateAlertCommand(string UserId, CreateStockAlertRequest Request) : IRequest<Result<StockAlertDto>>;

    public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, Result<StockAlertDto>>
    {
        private readonly IStockAlertService _alertService;
        private readonly ILogger<CreateAlertCommandHandler> _logger;

        public CreateAlertCommandHandler(IStockAlertService alertService, ILogger<CreateAlertCommandHandler> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        public async Task<Result<StockAlertDto>> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _alertService.CreateAlertAsync(request.UserId, request.Request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}