using StreamFlix.Models.Shelves;

namespace StreamFlix.Services.Shelves
{
    public class ShelvesService : IShelvesService
    {
        /*
            1. 📋 Fetches layout configuration from the Layout Service
            2. 🔍 Retrieves data from appropriate downstream services based on the configured datasource type in each shelf
            3. ✨ Enriches show IDs with metadata from the Video Library Service
            4. 🚀 Returns complete shelves data for frontend rendering
        */

        public ShelvesService()
        {

        }

        public ICollection<Shelf> GetShelves()
        {
            throw new NotImplementedException();
        }
    }
}