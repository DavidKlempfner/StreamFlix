namespace StreamFlix.Models.Layout
{
    public class LayoutConfig
    {
        public string Page { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public List<LayoutItem> Layout { get; set; } = [];
    }
}