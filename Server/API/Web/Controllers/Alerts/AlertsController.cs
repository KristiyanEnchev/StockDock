namespace Web.Controllers.Alerts
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Models.Stock;

    using Application.Interfaces.Identity;
    using Application.Handlers.Alerts.Commands;
    using Application.Handlers.Alerts.Queries;

    using Web.Extensions;

    [Authorize]
    public class AlertsController : ApiController
    {
        private readonly IUser _currentUser;

        public AlertsController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<ActionResult> GetAlerts()
        {
            return await Mediator.Send(new GetUserAlertsQuery(_currentUser.Id!)).ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAlert([FromBody] CreateStockAlertRequest request)
        {
            return await Mediator.Send(new CreateAlertCommand(_currentUser.Id!, request)).ToActionResult();
        }

        [HttpDelete("{alertId}")]
        public async Task<ActionResult> DeleteAlert(string alertId)
        {
            return await Mediator.Send(new DeleteAlertCommand( _currentUser.Id!, alertId)).ToActionResult();
        }
    }
}