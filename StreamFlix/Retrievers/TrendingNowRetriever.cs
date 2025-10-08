using StreamFlix.Models.Shelves;
using StreamFlix.Services.Recommendations;

namespace StreamFlix.Retrievers
{
    public class TrendingNowRetriever(IRecommendationsService recommendationsService) : IDataSourceRetriever
    {
        private readonly IRecommendationsService _recommendationsService = recommendationsService;

        public DataSourceType SupportedType => DataSourceType.TrendingNow;

        public Task<IList<string>> RetrieveShowIdsAsync()
        {
            return _recommendationsService.GetTrendingNowShowIdsAsync();
        }
    }
}