namespace Models.Dashboard
{
    public class SaveDashboardLayoutRequest
    {
        public List<SaveWidgetRequest> Widgets { get; set; } = new();
        public string Theme { get; set; } = "light";
    }
}
