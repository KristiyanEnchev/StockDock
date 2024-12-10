namespace Web.Controllers.Watchlist
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Models.Stock;

    using Shared;

    using Web.Extensions;

    using Application.Handlers.Watchlist.Commands;
    using Application.Handlers.Watchlist.Queries;
    using Application.Interfaces.Identity;

    [Authorize]
    public class WatchlistController : ApiController
    {
        private readonly IUser _currentUser;

        public WatchlistController(IUser currentUser)
        {
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<ActionResult<Result<IReadOnlyList<UserWatchlistDto>>>> GetMyWatchlist()
        {
            return await Mediator.Send(new GetUserWatchlistQuery(_currentUser.Id!))
                .ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<Result<UserWatchlistDto>>> AddToWatchlist(
            [FromBody] CreateWatchlistItemRequest request)
        {
            return await Mediator.Send(new AddToWatchlistCommand(_currentUser.Id!, request))
                .ToActionResult();
        }

        [HttpDelete("{symbol}")]
        public async Task<ActionResult<Result<bool>>> RemoveFromWatchlist(string symbol)
        {
            return await Mediator.Send(new RemoveFromWatchlistCommand(_currentUser.Id!, symbol))
                .ToActionResult();
        }
    }
}