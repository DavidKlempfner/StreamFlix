using StreamFlix.Models.Layout;
using StreamFlix.Models.Shelves;
using StreamFlix.Models.Shows;
using StreamFlix.Services.Layout;
using StreamFlix.Services.Recommendations;
using StreamFlix.Services.VideoLibrary;
using StreamFlix.Mappers;

namespace StreamFlix.Services.Shelves
{
    public class ShelvesService(ILayoutService layoutService, IRecommendationsService recommendationsService, IVideoLibraryService videoLibraryService) : IShelvesService
    {
        /*
            1. 📋 Fetches layout configuration from the Layout Service
            2. 🔍 Retrieves data from appropriate downstream services based on the configured datasource type in each shelf
            3. ✨ Enriches show IDs with metadata from the Video Library Service
            4. 🚀 Returns complete shelves data for frontend rendering
        */

        private readonly List<DataSourceType> _personalisedDataSourceTypes = [DataSourceType.ContinuePlaying];

        // Different methods can be added here in the future to handle different DataSourceTypes
        // This is to avoid adding a new if statement in GetShelves() every time a new DataSourceType is added
        private readonly Dictionary<DataSourceType, Func<Task<IList<string>>>> dataSourceTypeRetrieverMappers = new()
        {
            { DataSourceType.TrendingNow, recommendationsService.GetTrendingNowShowIdsAsync }
        };

        private readonly Dictionary<ShelfType, Func<LayoutItem, IList<Show>, IList<ShelfItem>>> shelfMappers = new()
        {
            { ShelfType.ShowsShelf, (layoutItem, shows) => ShelfItemMapper.MapLayoutItemAndShowsToShowsShelf(layoutItem, shows).Cast<ShelfItem>().ToList() },
            { ShelfType.HeaderShelf, (layoutItem, shows) => ShelfItemMapper.MapLayoutItemAndShowsToHeaderShelf(layoutItem, shows).Cast<ShelfItem>().ToList() }
        };

        public async Task<List<List<ShelfItem>>> GetNonPersonalisedShelves()
        {
            var layoutConfig = await layoutService.GetHomePageLayoutAsync();
            /* eg:
             {
                "page": "home",
                "title": "Home Page",
                "layout": [
                    {
                        "id": "5467",
                        "type": "HeaderShelf",
                        "datasourceType": "TrendingNow"
                    },
                    {
                        "id": "1290",
                        "type": "ShowsShelf",
                        "title": "Trending Now",
                        "maxItems": 10,
                        "datasourceType": "TrendingNow"
                    },
                    {
                        "id": "1290",
                        "type": "ShowsShelf",
                        "title": "Trending Yesterday",
                        "maxItems": 7,
                        "datasourceType": "TrendingYesterday"
                    },
                    {
                        "id": "3498",
                        "type": "ShowsShelf",
                        "title": "Continue Watching",
                        "maxItems": 10,
                        "datasourceType": "ContinuePlaying"
                    }
                ]
            }
            */

            var nonPersonalisedShelves = await GetNonPersonalisedShelvesFromConfigAsync(layoutConfig);

            return nonPersonalisedShelves;

            // TODO: return nonPersonalisedShelves AND the URLs needed for the front-end to call the personalised shelves that display below the fold.
            // The trade off is an extra HTTP request but a faster above the fold load time.
            // Once the app is opened, the HTTP request can be made to display the non personalised data above the fold very quickly (definitely use caching).
            // The front-end can then make a second HTTP request to get the personalised data below the fold using the URLs provided from the initial response.
        }

        private async Task<List<List<ShelfItem>>> GetNonPersonalisedShelvesFromConfigAsync(LayoutConfig layoutConfig)
        {            
            var nonPersonalisedShelfLayoutItems = layoutConfig.Layout.Where(item => !IsPersonalisedShelf(item.DataSourceType));
            //eg:
            /*
             {
                        "id": "5467",
                        "type": "HeaderShelf",
                        "datasourceType": "TrendingNow"
                    },
                    {
                        "id": "1290",
                        "type": "ShowsShelf",
                        "title": "Trending Now",
                        "maxItems": 10,
                        "datasourceType": "TrendingNow"
                    },
                    {
                        "id": "1290",
                        "type": "ShowsShelf",
                        "title": "Trending Yesterday",
                        "maxItems": 7,
                        "datasourceType": "TrendingYesterday"
                    }
             */

            var nonPersonalisedDataSourceTypes = nonPersonalisedShelfLayoutItems.Select(shelf => shelf.DataSourceType).Distinct();
            // eg: [ TrendingNow, TrendingYesterday ]

            var nonPersonalisedDataSourceTypesAndShowIds = await GetDataSourceTypesToShowIdsMappingAsync(nonPersonalisedDataSourceTypes);
            // eg:
            // { TrendingNow:       [ "show001", "show002" ] },
            // { TrendingYesterday: [ "show003", "show004" ] }

            var nonPersonalisedDataSourceTypesAndShows = await GetDataSourceTypesToShowsMappingAsync(nonPersonalisedDataSourceTypesAndShowIds);
            // eg:
            //  { TrendingNow:       [ { id: "show001", title: "The Great Adventure" }, { id: "show002", title: "Comedy Central" } ] },
            //  { TrendingYesterday: [ { id: "show003", title: "Space Odyssey" }, { id: "show004", title: "Historical Chronicles" } ] }

            var nonPersonalisedShelves = ConvertToNonPersonalisedShelves(nonPersonalisedShelfLayoutItems, nonPersonalisedDataSourceTypesAndShows);

            return nonPersonalisedShelves;
        }

        private List<List<ShelfItem>> ConvertToNonPersonalisedShelves(IEnumerable<LayoutItem> nonPersonalisedShelfLayoutItems, Dictionary<DataSourceType, IList<Show>> nonPersonalisedDataSourceTypesAndShows)
        {
            List<List<ShelfItem>> nonPersonalisedShelves = new();
            foreach (var shelfLayoutItem in nonPersonalisedShelfLayoutItems)
            {
                var showsForShelf = nonPersonalisedDataSourceTypesAndShows[shelfLayoutItem.DataSourceType];
                var shelfItems = shelfMappers[shelfLayoutItem.Type].Invoke(shelfLayoutItem, showsForShelf);
                nonPersonalisedShelves.Add(shelfItems.ToList());
            }

            return nonPersonalisedShelves;
        }

        private async Task<IList<Show>> GetShowsAsync(IList<string> showIds)
        {
            var tasks = showIds
                .Select(showId => Task.Run(async () =>
                {
                    var shows = await videoLibraryService.GetShowMetadataAsync(showId);
                    return shows;
                }))
                .ToList();

            return await Task.WhenAll(tasks);
        }

        private async Task<Dictionary<DataSourceType, IList<Show>>> GetDataSourceTypesToShowsMappingAsync(Dictionary<DataSourceType, IList<string>> nonPersonalisedDataSourceTypesAndShowIds)
        {
            var dataSouceTypeToShows = new Dictionary<DataSourceType, IList<Show>>();

            foreach (var (dataSourceType, showIds) in nonPersonalisedDataSourceTypesAndShowIds)
            {
                // TODO: consider running these in parallel
                var shows = await GetShowsAsync(showIds);
                dataSouceTypeToShows[dataSourceType] = shows;
            }

            return dataSouceTypeToShows;
        }

        private async Task<Dictionary<DataSourceType, IList<string>>> GetDataSourceTypesToShowIdsMappingAsync(IEnumerable<DataSourceType> dataSourceTypes)
        {
            // Run all tasks in parallel to improve loading for the current user.
            // But consider this will use up threads from the thread pool that could be used to handle other incoming requests.
            // TODO: consider this tradeoff
            var tasks = dataSourceTypes
                .Select(type => Task.Run(async () =>
                {
                    var showIds = await dataSourceTypeRetrieverMappers[type].Invoke();
                    return (type, showIds);
                }))
                .ToList();

            var results = await Task.WhenAll(tasks);
            return results.ToDictionary(result => result.type, result => result.showIds);
        }

        private bool IsPersonalisedShelf(DataSourceType datasourceType)
        {
            return _personalisedDataSourceTypes.Contains(datasourceType);
        }
    }
}