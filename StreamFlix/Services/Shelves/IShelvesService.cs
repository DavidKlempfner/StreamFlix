using StreamFlix.Models.Shelves;

namespace StreamFlix.Services.Shelves
{
    public interface IShelvesService
    {
        ICollection<Shelf> GetShelves();
    }
}