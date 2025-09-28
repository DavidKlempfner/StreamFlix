using System.Text.Json;
using StreamFlix.Models.Layout;

namespace StreamFlix.Services.Layout
{
    public class LayoutService(HttpClient httpClient) : ILayoutService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<LayoutConfig> GetHomePageLayoutAsync()
        {
            var response = await _httpClient.GetAsync("/layout/api/page/home");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var layoutConfig = JsonSerializer.Deserialize<LayoutConfig>(json);
            return layoutConfig;
        }
    }
}