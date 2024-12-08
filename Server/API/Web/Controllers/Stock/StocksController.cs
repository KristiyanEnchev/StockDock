namespace Web.Controllers.Stock
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Models.Stock;

    using Shared;

    using Web.Extensions;

    using Application.Handlers.Stocks.Commands;
    using Application.Handlers.Stocks.Queries;

    [Authorize]
    public class StocksController : ApiController
    {
        [HttpGet("{symbol}")]
        public async Task<ActionResult<Result<StockDto>>> GetBySymbol(string symbol)
        {
            return await Mediator.Send(new GetStockBySymbolQuery(symbol))
                .ToActionResult();
        }

        [HttpPut("{symbol}/price")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Result<StockDto>>> UpdatePrice(
            string symbol,
            [FromBody] decimal newPrice)
        {
            return await Mediator.Send(new UpdateStockPriceCommand(symbol, newPrice))
                .ToActionResult();
        }
    }
}