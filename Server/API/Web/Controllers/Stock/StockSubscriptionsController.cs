namespace Web.Controllers.Stock
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Shared;

    using Web.Extensions;
    using Application.Handlers.Subscriptions.Commands;
    using Application.Handlers.Subscriptions.Queries;
    using Application.Interfaces.Identity;

    [Authorize]
    public class StockSubscriptionsController : ApiController
    {
        private readonly IUser _currentUser;

        public StockSubscriptionsController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<ActionResult<Result<IReadOnlyList<string>>>> GetMySubscriptions()
        {
            return await Mediator.Send(new GetActiveSubscriptionsQuery(_currentUser.Id!))
                .ToActionResult();
        }

        [HttpPost("{symbol}/subscribe")]
        public async Task<ActionResult<Result<bool>>> Subscribe(string symbol)
        {
            return await Mediator.Send(new SubscribeToStockCommand(_currentUser.Id!, symbol))
                .ToActionResult();
        }

        [HttpPost("{symbol}/unsubscribe")]
        public async Task<ActionResult<Result<bool>>> Unsubscribe(string symbol)
        {
            return await Mediator.Send(new UnsubscribeFromStockCommand(_currentUser.Id!, symbol))
                .ToActionResult();
        }
    }
}
