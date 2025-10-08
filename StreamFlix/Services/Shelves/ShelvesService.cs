using StreamFlix.Models.Layout;
using StreamFlix.Models.Shelves;
using StreamFlix.Models.Shows;
using StreamFlix.Services.Layout;
using StreamFlix.Services.Recommendations;
using StreamFlix.Services.VideoLibrary;
using StreamFlix.Mappers;
using StreamFlix.Retrievers;

namespace StreamFlix.Services.Shelves
{
    public class ShelvesService(ILayoutService layoutService,
        IVideoLibraryService videoLibraryService,
        IEnumerable<IShelfMapper> shelfMappers,
        IEnumerable<IDataSourceRetriever> dataSourceRetrievers) : IShelvesService
    {
        /*
            1. 📋 Fetches layout configuration from the Layout Service
            2. 🔍 Retrieves data from appropriate downstream services based on the configured datasource type in each shelf
            3. ✨ Enriches show IDs with metadata from the Video Library Service
            4. 🚀 Returns complete shelves data for frontend rendering
        */

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
            // TODO: consider calling ToList() to enforce just one enumeration of the IEnumerable.
            // At the moment it's being enumerated twice (see .Select() below and foreach loop in ConvertToNonPersonalisedShelves)
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

        private List<List<ShelfItem>> ConvertToNonPersonalisedShelves(IEnumerable<LayoutItem> nonPersonalisedShelfLayoutItems,
            Dictionary<DataSourceType, IList<Show>> nonPersonalisedDataSourceTypesAndShows)
        {
            List<List<ShelfItem>> nonPersonalisedShelves = new();
            foreach (var shelfLayoutItem in nonPersonalisedShelfLayoutItems)
            {
                var showsForShelf = nonPersonalisedDataSourceTypesAndShows[shelfLayoutItem.DataSourceType];
                var shelfMapper = shelfMappers.Single(x => x.SupportedType == shelfLayoutItem.Type);
                var shelfItems = shelfMapper.MapToShelfItems(shelfLayoutItem, showsForShelf);
                nonPersonalisedShelves.Add(shelfItems.ToList());
            }

            return nonPersonalisedShelves;
        }

        private async Task<IList<Show>> GetShowsAsync(IList<string> showIds)
        {
            // I used AI to generate this however after looking at it further I think this would have been a better option:
            /*
             var tasks = showIds
                .Select(showId => videoLibraryService.GetShowMetadataAsync(showId))
                .ToList();
             */
            // ToList() enumerates the IEnumerable and will run the GetShowMetadataAsync() calls.
            // HTTP calls are I/O bound, not CPU bound, so they should use async/await not Task.Run().
            // The problem with Task.Run() is a new thread is created and the thread just sits there waiting for the HTTP call to finish.
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
                    var dataSourceTypeRetrieverMapper = dataSourceRetrievers.Single(x => x.SupportedType == type);
                    var showIds = await dataSourceTypeRetrieverMapper.RetrieveShowIdsAsync();
                    return (type, showIds);
                }))
                .ToList();

            var results = await Task.WhenAll(tasks);
            return results.ToDictionary(result => result.type, result => result.showIds);
        }

        private bool IsPersonalisedShelf(DataSourceType datasourceType)
        {
            return dataSourceRetrievers.Single(x => x.SupportedType == datasourceType).IsPersonalised;
        }
    }
}