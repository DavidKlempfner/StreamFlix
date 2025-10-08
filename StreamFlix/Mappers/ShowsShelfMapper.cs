using StreamFlix.Models.Layout;
using StreamFlix.Models.Shelves;
using StreamFlix.Models.Shows;

namespace StreamFlix.Mappers
{
    public class ShowsShelfMapper : IShelfMapper
    {
        public ShelfType SupportedType => ShelfType.ShowsShelf;

        public IList<ShelfItem> MapToShelfItems(LayoutItem layoutItem, IList<Show> shows)
        {
            return MapLayoutItemAndShowsToShowsShelf(layoutItem, shows).Cast<ShelfItem>().ToList();
        }

        private static IList<ShowsShelfItem> MapLayoutItemAndShowsToShowsShelf(LayoutItem layoutItem, IList<Show> shows)
        {
            return shows.Take(layoutItem.MaxItems ?? shows.Count)
                .Select(MapShowToShowsShelfItem)
                .ToList();
        }

        private static ShowsShelfItem MapShowToShowsShelfItem(Show show)
        {
            return new ShowsShelfItem
            {
                Title = show.Title,
                PlaybackUrl = show.PlaybackUrl,
                ShelfType = ShelfType.ShowsShelf,
                ThumbnailUrl = show.ThumbnailUrl
            };
        }
    }
}