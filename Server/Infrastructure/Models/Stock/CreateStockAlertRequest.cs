namespace Models.Stock
{
    using Domain.Entities.Stock;

    public class CreateStockAlertRequest
    {
        public string Symbol { get; set; } = string.Empty;
        public AlertType Type { get; set; }
        public decimal Threshold { get; set; }
    }
}