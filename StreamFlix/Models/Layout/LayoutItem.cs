namespace StreamFlix.Models.Layout
{
    public class LayoutItem
    {
        public string Id { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string DatasourceType { get; set; } = string.Empty;

        public string? Title { get; set; }

        public int? MaxItems { get; set; }
    }
}