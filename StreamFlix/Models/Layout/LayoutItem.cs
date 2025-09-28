using System.Text.Json.Serialization;
using StreamFlix.Models.Shelves;

namespace StreamFlix.Models.Layout
{
    public class LayoutItem
    {
        public string Id { get; set; } = string.Empty;

        public ShelfType Type { get; set; }

        public DataSourceType DataSourceType { get; set; }

        public string? Title { get; set; }

        public int? MaxItems { get; set; }
    }
}

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
      "id": "3498",
      "type": "ShowsShelf",
      "title": "Continue Watching",
      "maxItems": 10,
      "datasourceType": "ContinuePlaying"
    }
  ]
}
 */