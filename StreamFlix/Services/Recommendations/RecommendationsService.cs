using System.Text.Json;

namespace StreamFlix.Services.Recommendations
{
    public class RecommendationsService(HttpClient httpClient) : IRecommendationsService
    {
        private readonly HttpClient _httpClient = httpClient;

        // TODO: pass through maxItems to limit the number of results returned to increase performance
        public async Task<IList<string>> GetTrendingNowShowIdsAsync()
        {
            var response = await _httpClient.GetAsync("/recommendations/api/trending");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IList<string>>(json);
        }
    }
}