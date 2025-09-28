namespace StreamFlix.Models.Shelves
{
    public class ShelfItem
    {
        public ShelfType ShelfType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PlaybackUrl { get; set; } = string.Empty;
    }
}