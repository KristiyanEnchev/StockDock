namespace Models.Dashboard
{
    public class DashboardLayoutDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<WidgetDto> Widgets { get; set; } = new();
        public string Theme { get; set; } = "light";
        public DateTime LastUpdated { get; set; }
    }
}
