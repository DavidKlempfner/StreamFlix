using System.Text.Json;
using StreamFlix.Models.Layout;

namespace StreamFlix.Services.Layout
{
    public class LayoutService : ILayoutService
    {
        private readonly HttpClient _httpClient;

        public LayoutService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LayoutConfig> GetHomePageLayoutAsync()
        {
            var response = await _httpClient.GetAsync("/layout/api/page/home");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LayoutConfig>(json);
        }
    }
}