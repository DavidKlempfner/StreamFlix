using StreamFlix.Models.Layout;
using StreamFlix.Models.Shelves;
using StreamFlix.Models.Shows;

namespace StreamFlix.Mappers
{
    public class HeaderShelfMapper : IShelfMapper
    {
        public ShelfType SupportedType => ShelfType.HeaderShelf;

        public IList<ShelfItem> MapToShelfItems(LayoutItem layoutItem, IList<Show> shows)
        {
            return MapLayoutItemAndShowsToHeaderShelf(layoutItem, shows).Cast<ShelfItem>().ToList();
        }

        private static IList<HeaderShelfItem> MapLayoutItemAndShowsToHeaderShelf(LayoutItem layoutItem, IList<Show> shows)
        {
            return [MapShowToHeaderShelfItem(shows[0])];
        }

        private static HeaderShelfItem MapShowToHeaderShelfItem(Show show)
        {
            return new HeaderShelfItem
            {
                Title = show.Title,
                PlaybackUrl = show.PlaybackUrl,
                ShelfType = ShelfType.ShowsShelf,
                BackgroundImageUrl = show.BackgroundImageUrl,
                Genre = show.Genre,
                DurationInSeconds = show.Duration,
                Rating = show.Rating
            };
        }
    }
}