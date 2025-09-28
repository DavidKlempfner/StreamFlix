using StreamFlix.Models.Layout;
using StreamFlix.Models.Shelves;
using StreamFlix.Models.Shows;
using StreamFlix.Services.Layout;
using StreamFlix.Services.Recommendations;
using StreamFlix.Services.VideoLibrary;

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
        private readonly Dictionary<DataSourceType, Func<Task<ICollection<string>>>> dataSourceTypeRetrievers = new()
        {
            { DataSourceType.TrendingNow, recommendationsService.GetTrendingNowShowIdsAsync }
        };

        public async Task<ICollection<Shelf>> GetShelves()
        {
            var layoutConfig = await layoutService.GetHomePageLayoutAsync();
            /*
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

            Dictionary<DataSourceType, ICollection<string>> dataSourceTypeToShowIds = new();

            var nonPersonalisedShelves = await GetNonPersonalisedShelvesAsync(layoutConfig);

            return nonPersonalisedShelves;
        }

        private async Task<ICollection<Shelf>> GetNonPersonalisedShelvesAsync(LayoutConfig layoutConfig)
        {            
            var nonPersonalisedShelfLayoutItems = layoutConfig.Layout.Where(item => !IsPersonalisedShelf(item.DatasourceType));
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

            var nonPersonalisedDataSourceTypes = nonPersonalisedShelfLayoutItems.Select(shelf => shelf.DatasourceType).Distinct();
            // eg: [ TrendingNow, TrendingYesterday ]

            var dataSourceTypeToShowIds = await GetDataSourceTypeToShowIdsMapping(nonPersonalisedDataSourceTypes);
            // eg:
            // { TrendingNow:       [ "showId1", "showId2", "showId3" ] },
            // { TrendingYesterday: [ "showId4", "showId5", "showId6" ] }

            var dataSouceTypeToShows = new Dictionary<DataSourceType, ICollection<Show>>();

            // foreach one, get the show metadata from the Video Library Service
            foreach (var dataSourceType in dataSourceTypeToShowIds.Keys)
            {
                var showIds = dataSourceTypeToShowIds[dataSourceType];
                var showRetrievalTasks = CreateShowRetrievalTasks(showIds.ToList());
                var shows = await Task.WhenAll(showRetrievalTasks);
                dataSouceTypeToShows.Add(dataSourceType, shows.SelectMany(s => s).ToList());
                // eg: [ Show { id="showId1", title="..." }, Show { id="showId2", title="..." }, Show { id="showId3", title="..." } ]
            }


            return [];
        }

        private async Task<Dictionary<DataSourceType, ICollection<string>>> GetDataSourceTypeToShowIdsMapping(IEnumerable<DataSourceType> nonPersonalisedDataSourceTypes)
        {
            // Run all tasks in parallel to improve performance
            // But consider this will use up threads from the thread pool that could be used to handle other incoming requests.
            // TODO: consider this tradeoff further and implement caching if necessary.
            var showIdRetrievalTasks = CreateShowIdRetrievalTasks(nonPersonalisedDataSourceTypes);
            var nonPersonalisedDataSourceTypesAndShowIds = await Task.WhenAll(showIdRetrievalTasks);

            Dictionary<DataSourceType, ICollection<string>> dataSourceTypeToShowIds = new();
            // eg: TrendingNow: [ "showId1", "showId2", "showId3" ]
            // eg. TrendingYesterday: [ "showId4", "showId5", "showId6" ]

            foreach (var (type, showIds) in nonPersonalisedDataSourceTypesAndShowIds)
            {
                dataSourceTypeToShowIds.Add(type, showIds);
            }

            return dataSourceTypeToShowIds;
        }

        private List<Task<ICollection<Show>>> CreateShowRetrievalTasks(List<string> showIds)
        {
            return showIds
                .Select(showId => Task.Run(async () =>
                {
                    var show = await videoLibraryService.GetShowMetadataAsync(showId);
                    return show;
                }))
                .ToList();
        }

        private List<Task<(DataSourceType, ICollection<string>)>> CreateShowIdRetrievalTasks(IEnumerable<DataSourceType> nonPersonalisedDataSourceTypes)
        {
            return nonPersonalisedDataSourceTypes
                .Select(type => Task.Run(async () =>
                {
                    var showIds = await dataSourceTypeRetrievers[type].Invoke();
                    return (type, showIds);
                }))
                .ToList();
        }

        private bool IsPersonalisedShelf(DataSourceType datasourceType)
        {
            return _personalisedDataSourceTypes.Contains(datasourceType);
        }
    }
}