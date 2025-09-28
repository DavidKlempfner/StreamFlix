using System.Text.Json.Serialization;

namespace StreamFlix.Models.Shelves
{
    public class HeaderShelfItem : ShelfItem
    {
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public int DurationInSeconds { get; set; }

        public string Rating { get; set; } = string.Empty;

        // Using BackgroundImageUrl as the property name to be consistent with other properties containing URLs
        [JsonPropertyName("backgroundImage")]
        public string BackgroundImageUrl { get; set; } = string.Empty;
    }
}