using StreamFlix.Models.Shows;

namespace StreamFlix.Services.VideoLibrary
{
    public interface IVideoLibraryService
    {
        Task<ICollection<Show>> GetShowMetadataAsync(string showId);
    }
}