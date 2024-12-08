namespace Domain.Entities
{
    public class UserStockSubscription : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string StockId { get; set; } = string.Empty;
        public Stock Stock { get; set; } = null!;
        public DateTime SubscribedAt { get; set; }
    }
}
