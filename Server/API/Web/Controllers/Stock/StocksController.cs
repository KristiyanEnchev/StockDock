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

        [HttpGet("{symbol}/history")]
        public async Task<ActionResult<Result<IReadOnlyList<StockPriceHistoryDto>>>> GetHistory(
            string symbol,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            return await Mediator.Send(new GetStockHistoryQuery(symbol, from, to))
                .ToActionResult();
        }

        [HttpGet("{symbol}/indicators")]
        public async Task<ActionResult<Result<TechnicalIndicatorsDto>>> GetIndicators(
            string symbol,
            [FromQuery] string[] indicators)
        {
            return await Mediator.Send(new GetTechnicalIndicatorsQuery(symbol, indicators))
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

        [HttpPost("sync")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Result<int>>> SyncExternalPrices()
        {
            return await Mediator.Send(new SyncExternalPricesCommand())
                .ToActionResult();
        }
    }
}