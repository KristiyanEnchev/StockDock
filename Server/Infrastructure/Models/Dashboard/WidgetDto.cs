namespace Models.Dashboard
{
    public class WidgetDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;  // chart, watchlist, alerts, etc.
        public string Title { get; set; } = string.Empty;
        public int X { get; set; }  // Grid position X
        public int Y { get; set; }  // Grid position Y
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new();
    }
}
