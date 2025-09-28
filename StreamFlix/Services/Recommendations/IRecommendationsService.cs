namespace StreamFlix.Services.Recommendations
{
    public interface IRecommendationsService
    {
        Task<ICollection<string>> GetTrendingNowShowIdsAsync();
    }
}