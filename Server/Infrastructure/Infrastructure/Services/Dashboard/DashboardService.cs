namespace Infrastructure.Services.Dashboard
{
    using System.Text.Json;

    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Models.Dashboard;

    using Shared;
    using Shared.Interfaces;

    using Application.Interfaces.Cache;
    using Application.Interfaces.Dashboard;

    public class DashboardService : IDashboardService
    {
        private readonly IRepository<DashboardLayout> _layoutRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DashboardService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public DashboardService(
            IRepository<DashboardLayout> layoutRepository,
            ICacheService cacheService,
            ILogger<DashboardService> logger,
            JsonSerializerOptions jsonOptions)
        {
            _layoutRepository = layoutRepository;
            _cacheService = cacheService;
            _logger = logger;
            _jsonOptions = jsonOptions;
        }

        public async Task<Result<DashboardLayoutDto>> GetLayoutAsync(string userId)
        {
            try
            {
                var cacheKey = $"dashboard:layout:{userId}";
                var cachedLayout = await _cacheService.GetAsync<DashboardLayoutDto>(cacheKey);
                if (cachedLayout != null)
                {
                    return Result<DashboardLayoutDto>.SuccessResult(cachedLayout);
                }

                var layout = await _layoutRepository
                    .AsNoTracking()
                    .Include(l => l.Widgets)
                    .FirstOrDefaultAsync(l => l.UserId == userId);

                if (layout == null)
                {
                    return Result<DashboardLayoutDto>.SuccessResult(CreateDefaultLayout(userId));
                }

                var layoutDto = new DashboardLayoutDto
                {
                    UserId = layout.UserId,
                    Theme = layout.Theme,
                    LastUpdated = layout.UpdatedDate ?? layout.CreatedDate ?? DateTime.UtcNow,
                    Widgets = layout.Widgets.Select(w => new WidgetDto
                    {
                        Id = w.Id,
                        Type = w.Type,
                        Title = w.Title,
                        X = w.X,
                        Y = w.Y,
                        Width = w.Width,
                        Height = w.Height,
                        Settings = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            w.Settings, _jsonOptions) ?? new Dictionary<string, object>()
                    }).ToList()
                };

                await _cacheService.SetAsync(cacheKey, layoutDto, TimeSpan.FromMinutes(5));
                return Result<DashboardLayoutDto>.SuccessResult(layoutDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard layout for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<bool>> SaveLayoutAsync(string userId, SaveDashboardLayoutRequest request)
        {
            try
            {
                var layout = await _layoutRepository
                    .AsTracking()
                    .Include(l => l.Widgets)
                    .FirstOrDefaultAsync(l => l.UserId == userId);

                if (layout == null)
                {
                    layout = new DashboardLayout
                    {
                        UserId = userId,
                        Theme = request.Theme
                    };
                    await _layoutRepository.AddAsync(layout);
                }
                else
                {
                    layout.Theme = request.Theme;
                    layout.Widgets.Clear();
                }

                foreach (var widgetRequest in request.Widgets)
                {
                    var widget = new Widget
                    {
                        Type = widgetRequest.Type,
                        Title = widgetRequest.Title,
                        X = widgetRequest.X,
                        Y = widgetRequest.Y,
                        Width = widgetRequest.Width,
                        Height = widgetRequest.Height,
                        Settings = JsonSerializer.Serialize(widgetRequest.Settings, _jsonOptions)
                    };
                    layout.Widgets.Add(widget);
                }

                await _layoutRepository.SaveChangesAsync();

                var cacheKey = $"dashboard:layout:{userId}";
                await _cacheService.RemoveAsync(cacheKey);

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving dashboard layout for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<bool>> SaveWidgetSettingsAsync(
            string userId,
            string widgetId,
            Dictionary<string, object> settings)
        {
            try
            {
                var layout = await _layoutRepository
                    .AsTracking()
                    .Include(l => l.Widgets)
                    .FirstOrDefaultAsync(l => l.UserId == userId && l.Widgets.Any(w => w.Id == widgetId));

                if (layout == null)
                {
                    return Result<bool>.Failure("Dashboard layout or widget not found.");
                }

                var widget = layout.Widgets.First(w => w.Id == widgetId);
                widget.Settings = JsonSerializer.Serialize(settings, _jsonOptions);

                await _layoutRepository.SaveChangesAsync();

                var cacheKey = $"dashboard:layout:{userId}";
                await _cacheService.RemoveAsync(cacheKey);

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error saving widget settings for user {UserId}, widget {WidgetId}",
                    userId, widgetId);
                throw;
            }
        }

        public async Task<Result<bool>> DeleteWidgetAsync(string userId, string widgetId)
        {
            try
            {
                var layout = await _layoutRepository
                    .AsTracking()
                    .Include(l => l.Widgets)
                    .FirstOrDefaultAsync(l => l.UserId == userId && l.Widgets.Any(w => w.Id == widgetId));

                if (layout == null)
                {
                    return Result<bool>.Failure("Dashboard layout or widget not found.");
                }

                var widget = layout.Widgets.First(w => w.Id == widgetId);
                layout.Widgets.Remove(widget);

                await _layoutRepository.SaveChangesAsync();

                var cacheKey = $"dashboard:layout:{userId}";
                await _cacheService.RemoveAsync(cacheKey);

                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deleting widget for user {UserId}, widget {WidgetId}",
                    userId, widgetId);
                throw;
            }
        }

        private DashboardLayoutDto CreateDefaultLayout(string userId)
        {
            return new DashboardLayoutDto
            {
                UserId = userId,
                Theme = "light",
                LastUpdated = DateTime.UtcNow,
                Widgets = new List<WidgetDto>
                {
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "watchlist",
                        Title = "My Watchlist",
                        X = 0,
                        Y = 0,
                        Width = 12,
                        Height = 6
                    },
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "chart",
                        Title = "Market Overview",
                        X = 0,
                        Y = 6,
                        Width = 8,
                        Height = 6,
                        Settings = new Dictionary<string, object>
                        {
                            ["symbol"] = "SPY",
                            ["interval"] = "1d",
                            ["indicators"] = new[] { "sma", "volume" }
                        }
                    },
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "alerts",
                        Title = "Price Alerts",
                        X = 8,
                        Y = 6,
                        Width = 4,
                        Height = 6
                    }
                }
            };
        }
    }
}
