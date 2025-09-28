using StreamFlix.Models.Shelves;

namespace StreamFlix.Services.Shelves
{
    public interface IShelvesService
    {
        Task<List<List<ShelfItem>>> GetNonPersonalisedShelves();
    }
}