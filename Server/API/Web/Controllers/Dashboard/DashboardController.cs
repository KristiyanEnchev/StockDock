namespace Web.Controllers.Dashboard
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Models.Dashboard;

    using Shared;

    using Web.Extensions;

    using Application.Interfaces.Identity;
    using Application.Handlers.Dashboard.Command;
    using Application.Handlers.Dashboard.Queries;

    [Authorize]
    public class DashboardController : ApiController
    {
        private readonly IUser _currentUser;

        public DashboardController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet("layout")]
        public async Task<ActionResult<Result<DashboardLayoutDto>>> GetMyLayout()
        {
            return await Mediator.Send(new GetDashboardLayoutQuery(_currentUser.Id!))
                .ToActionResult();
        }

        [HttpPost("layout")]
        public async Task<ActionResult<Result<bool>>> SaveLayout(
            [FromBody] SaveDashboardLayoutRequest request)
        {
            return await Mediator.Send(new SaveDashboardLayoutCommand(_currentUser.Id!, request))
                .ToActionResult();
        }

        [HttpPost("widgets/{widgetId}/settings")]
        public async Task<ActionResult<Result<bool>>> SaveWidgetSettings(
            string widgetId,
            [FromBody] Dictionary<string, object> settings)
        {
            return await Mediator.Send(new SaveWidgetSettingsCommand(_currentUser.Id!, widgetId, settings))
                .ToActionResult();
        }
    }
}