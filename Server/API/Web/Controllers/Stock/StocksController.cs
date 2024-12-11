namespace Web.Controllers.Stock
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Application.Handlers.Stocks.Queries;

    using Web.Extensions;

    [Authorize]
    public class StocksController : ApiController
    {
        [HttpGet("{symbol}")]
        public async Task<ActionResult> GetStock(string symbol)
        {
            return await Mediator.Send(new GetStockBySymbolQuery(symbol)).ToActionResult();
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchStocks(
            [FromQuery] string query,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true)
        {
            return await Mediator.Send(new SearchStocksQuery(query, sortBy, ascending)).ToActionResult();
        }

        [HttpGet("popular")]
        public async Task<ActionResult> GetPopularStocks([FromQuery] int limit = 10)
        {
            return await Mediator.Send(new GetPopularStocksQuery(limit)).ToActionResult();
        }

        [HttpGet("{symbol}/history")]
        public async Task<ActionResult> GetStockHistory(
            string symbol,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            return await Mediator.Send(new GetStockHistoryQuery(symbol, from, to)).ToActionResult();
        }
    }
}