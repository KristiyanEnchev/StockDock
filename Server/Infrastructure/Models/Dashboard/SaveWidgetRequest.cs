namespace Models.Dashboard
{
    public class SaveWidgetRequest
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new();
    }
}