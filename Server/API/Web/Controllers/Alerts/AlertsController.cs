namespace Web.Controllers.Alerts
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Shared;

    using Web.Extensions;

    using Models.Alerts;

    using Application.Interfaces.Identity;
    using Application.Handlers.Alerts.Commands;
    using Application.Handlers.Alerts.Queries;

    [Authorize]
    public class AlertsController : ApiController
    {
        private readonly IUser _currentUser;

        public AlertsController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<ActionResult<Result<IReadOnlyList<StockAlertDto>>>> GetMyAlerts()
        {
            return await Mediator.Send(new GetUserAlertsQuery(_currentUser.Id!))
                .ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<Result<StockAlertDto>>> CreateAlert(
            [FromBody] CreateStockAlertRequest request)
        {
            return await Mediator.Send(new CreateAlertCommand(_currentUser.Id!, request))
                .ToActionResult();
        }

        [HttpPut("{alertId}")]
        public async Task<ActionResult<Result<StockAlertDto>>> UpdateAlert(
            string alertId,
            [FromBody] UpdateStockAlertRequest request)
        {
            return await Mediator.Send(new UpdateAlertCommand(_currentUser.Id!, alertId, request))
                .ToActionResult();
        }

        [HttpDelete("{alertId}")]
        public async Task<ActionResult<Result<bool>>> DeleteAlert(string alertId)
        {
            return await Mediator.Send(new DeleteAlertCommand(_currentUser.Id!, alertId))
                .ToActionResult();
        }
    }
}
