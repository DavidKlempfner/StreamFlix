using StreamFlix.Models.Layout;
using StreamFlix.Models.Shelves;
using StreamFlix.Models.Shows;

namespace StreamFlix.Mappers
{
    public static class ShelfItemMapper
    {
        public static IList<ShowsShelfItem> MapLayoutItemAndShowsToShowsShelf(LayoutItem layoutItem, IList<Show> shows)
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

        public static IList<HeaderShelfItem> MapLayoutItemAndShowsToHeaderShelf(LayoutItem layoutItem, IList<Show> shows)
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