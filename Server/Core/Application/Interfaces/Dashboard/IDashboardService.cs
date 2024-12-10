namespace Application.Interfaces.Dashboard
{
    using Models.Dashboard;

    using Shared;

    public interface IDashboardService
    {
        Task<Result<DashboardLayoutDto>> GetLayoutAsync(string userId);
        Task<Result<bool>> SaveLayoutAsync(string userId, SaveDashboardLayoutRequest request);
        Task<Result<bool>> SaveWidgetSettingsAsync(string userId, string widgetId, Dictionary<string, object> settings);
        Task<Result<bool>> DeleteWidgetAsync(string userId, string widgetId);
    }
}