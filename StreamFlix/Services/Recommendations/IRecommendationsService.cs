namespace StreamFlix.Services.Recommendations
{
    public interface IRecommendationsService
    {
        Task<IList<string>> GetTrendingNowShowIdsAsync();
    }
}