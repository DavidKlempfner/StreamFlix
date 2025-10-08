using StreamFlix.Models.Shelves;
using StreamFlix.Services.Recommendations;

namespace StreamFlix.Retrievers
{
    public class TrendingNowRetriever(IRecommendationsService recommendationsService) : IDataSourceRetriever
    {
        private readonly IRecommendationsService _recommendationsService = recommendationsService;

        public bool IsPersonalised => false;
        public DataSourceType SupportedType => DataSourceType.TrendingNow;

        public async Task<IList<string>> RetrieveShowIdsAsync()
        {
            return await _recommendationsService.GetTrendingNowShowIdsAsync();
        }
    }
}