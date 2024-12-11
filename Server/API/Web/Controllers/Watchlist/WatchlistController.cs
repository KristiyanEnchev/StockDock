namespace Web.Controllers.Watchlist
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Application.Handlers.Watchlist.Commands;
    using Application.Handlers.Watchlist.Queries;

    using Web.Extensions;

    [Authorize]
    public class WatchlistController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult> GetWatchlist()
        {
            return await Mediator.Send(new GetUserWatchlistQuery()).ToActionResult();
        }

        [HttpPost("{symbol}")]
        public async Task<ActionResult> AddToWatchlist(string symbol)
        {
            return await Mediator.Send(new AddToWatchlistCommand(symbol)).ToActionResult();
        }

        [HttpDelete("{symbol}")]
        public async Task<ActionResult> RemoveFromWatchlist(string symbol)
        {
            return await Mediator.Send(new RemoveFromWatchlistCommand(symbol)).ToActionResult();
        }
    }
}