namespace Application.Handlers.Dashboard.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Dashboard;
    using Shared;
    using Application.Interfaces.Dashboard;

    public record GetDashboardLayoutQuery(string UserId) : IRequest<Result<DashboardLayoutDto>>;

    public class GetDashboardLayoutQueryHandler
        : IRequestHandler<GetDashboardLayoutQuery, Result<DashboardLayoutDto>>
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<GetDashboardLayoutQueryHandler> _logger;

        public GetDashboardLayoutQueryHandler(
            IDashboardService dashboardService,
            ILogger<GetDashboardLayoutQueryHandler> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<Result<DashboardLayoutDto>> Handle(
            GetDashboardLayoutQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _dashboardService.GetLayoutAsync(request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard layout for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}