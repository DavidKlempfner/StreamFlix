/*
 * Notes:
 * The home page experience is critical:
 * Users expect instant loading of above-the-fold content while personalized recommendations load seamlessly below.
 * However, Data Source Type handling should be abstracted from the front-end platforms, which should only be aware of how to render Shelf types.
 * In other words, the data source types should be resolved by the back end.
 * 
 * Ideally I'd like to have a GetTopShelves action method that returns the non-personalised shelves which appear above the fold.
 * And a GetPersonalisedShelves action method that returns the personalised shelves which appear below the fold.
 * But, datasourceType is what determines if it's personalised or not, and the front-end is unaware of this concept.
 * eg. layouts with datasourceType=TrendingNow and datasourceType=ContinuePlaying both have type=ShowsShelf.
 * 
 * Tradeoffs:
 * Robustness vs Correctness (from Code Complete by Steve McConnell):
 * If a down stream service is broken, it's better to return cached data than to show an error message (ie. prioritise robustness over correctness).
 */

using Microsoft.AspNetCore.Mvc;
using StreamFlix.Models.Shelves;
using StreamFlix.Services.Shelves;

namespace StreamFlix.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShelvesController(IShelvesService shelvesService) : ControllerBase
    {
        [HttpGet]
        public async Task<IList<ShelfItem>> GetShelves()
        {
            return await shelvesService.GetNonPersonalisedShelves();
        }
    }
}