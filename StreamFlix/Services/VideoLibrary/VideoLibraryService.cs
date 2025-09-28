using System.Text.Json;
using StreamFlix.Models.Shows;

namespace StreamFlix.Services.VideoLibrary
{
    public class VideoLibraryService(HttpClient httpClient) : IVideoLibraryService
    {
        private readonly HttpClient _httpClient = httpClient;

        // TODO: update to accept multiple showIds to reduce number of HTTP calls
        public async Task<ICollection<Show>> GetShowMetadataAsync(string showId)
        {
            var response = await _httpClient.GetAsync($"/shows/api/shows/{showId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ICollection<Show>>(json);
        }
    }
}