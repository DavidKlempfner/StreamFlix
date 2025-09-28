using StreamFlix.Models.Layout;

namespace StreamFlix.Services.Layout
{
    public interface ILayoutService
    {
        Task<LayoutConfig> GetHomePageLayoutAsync();
    }
}