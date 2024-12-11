namespace Domain.Entities.Stock
{
    using Domain.Entities.Base;
    using Domain.Entities.Identity;

    public enum AlertType
    {
        PriceAbove,
        PriceBelow,
        PercentageChange
    }

    public class StockAlert : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
        public AlertType Type { get; set; }
        public decimal Threshold { get; set; }
        public bool IsTriggered { get; set; } = false;
        public DateTime? LastTriggeredAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
    }
}
