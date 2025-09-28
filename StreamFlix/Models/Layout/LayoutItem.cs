using StreamFlix.Models.Shelves;

namespace StreamFlix.Models.Layout
{
    public class LayoutItem
    {
        public string Id { get; set; } = string.Empty;

        public ShelfType ShelfType { get; set; }

        public DataSourceType DatasourceType { get; set; }

        public string? Title { get; set; }

        public int? MaxItems { get; set; }
    }
}