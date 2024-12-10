namespace Models.Dashboard
{
    using Domain.Entities;

    public class Widget : BaseAuditableEntity
    {
        public string DashboardLayoutId { get; set; } = string.Empty;
        public DashboardLayout DashboardLayout { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Settings { get; set; } = "{}"; 
    }
}