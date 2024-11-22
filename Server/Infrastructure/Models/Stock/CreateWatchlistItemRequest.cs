namespace Models.Stock
{
    public class CreateWatchlistItemRequest
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal? AlertAbove { get; set; }
        public decimal? AlertBelow { get; set; }
    }
}
