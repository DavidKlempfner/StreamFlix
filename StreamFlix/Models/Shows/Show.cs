using System.Text.Json.Serialization;

namespace StreamFlix.Models.Shows
{
    public class Show
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public int Duration { get; set; }

        public string Rating { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ThumbnailUrl { get; set; } = string.Empty;

        public string PlaybackUrl { get; set; } = string.Empty;

        // Using BackgroundImageUrl as the property name to be consistent with other properties containing URLs
        [JsonPropertyName("backgroundImage")]
        public string BackgroundImageUrl { get; set; } = string.Empty;

        public int ReleaseYear { get; set; }
    }
}