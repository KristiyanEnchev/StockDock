namespace Application.Handlers.Dashboard.Command
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;
    using Application.Interfaces.Dashboard;

    public record SaveWidgetSettingsCommand(
        string UserId,
        string WidgetId,
        Dictionary<string, object> Settings) : IRequest<Result<bool>>;

    public class SaveWidgetSettingsCommandHandler
        : IRequestHandler<SaveWidgetSettingsCommand, Result<bool>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<SaveWidgetSettingsCommandHandler> _logger;

        public SaveWidgetSettingsCommandHandler(
            IDashboardService dashboardService,
            ILogger<SaveWidgetSettingsCommandHandler> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            SaveWidgetSettingsCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _dashboardService.SaveWidgetSettingsAsync(
                    request.UserId,
                    request.WidgetId,
                    request.Settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error saving widget settings for user {UserId}, widget {WidgetId}",
                    request.UserId, request.WidgetId);
                throw;
            }
        }
    }
}