using StreamFlix.Models.Layout;
using StreamFlix.Models.Shelves;
using StreamFlix.Models.Shows;

namespace StreamFlix.Mappers
{
    public interface IShelfMapper
    {
        ShelfType SupportedType { get; }
        IList<ShelfItem> MapToShelfItems(LayoutItem layoutItem, IList<Show> shows);
    }
}