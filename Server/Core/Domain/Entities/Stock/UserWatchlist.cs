namespace Domain.Entities.Stock
{
    using Domain.Entities.Base;
    using Domain.Entities.Identity;

    public class UserWatchlist : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public virtual User User { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
    }
}