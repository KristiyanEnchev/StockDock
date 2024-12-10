namespace Application.Handlers.Dashboard.Command
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Dashboard;

    using Shared;
    using Application.Interfaces.Dashboard;

    public record SaveDashboardLayoutCommand(string UserId, SaveDashboardLayoutRequest Layout)
        : IRequest<Result<bool>>;

    public class SaveDashboardLayoutCommandHandler
        : IRequestHandler<SaveDashboardLayoutCommand, Result<bool>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<SaveDashboardLayoutCommandHandler> _logger;

        public SaveDashboardLayoutCommandHandler(
            IDashboardService dashboardService,
            ILogger<SaveDashboardLayoutCommandHandler> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            SaveDashboardLayoutCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _dashboardService.SaveLayoutAsync(request.UserId, request.Layout);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving dashboard layout for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}