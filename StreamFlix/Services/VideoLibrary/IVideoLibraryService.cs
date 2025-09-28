using StreamFlix.Models.Shows;

namespace StreamFlix.Services.VideoLibrary
{
    public interface IVideoLibraryService
    {
        Task<Show> GetShowMetadataAsync(string showId);
    }
}