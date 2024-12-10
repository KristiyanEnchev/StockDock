namespace Models.Dashboard
{
    using Domain.Entities;

    public class DashboardLayout : BaseAuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public List<Widget> Widgets { get; set; } = new();
        public string Theme { get; set; } = "light";
    }
}