/*
 * Notes:
 * The home page experience is critical:
 * Users expect instant loading of above-the-fold content while personalized recommendations load seamlessly below.
 * However, Data Source Type handling should be abstracted from the front-end platforms, which should only be aware of how to render Shelf types.
 * In other words, the data source types should be resolved by the back end.
 * 
 * Questions:
 * If the layout is only known by ShelvesService once it makes a call to LayoutService,
 * how can the front-end know to quickly load just the nonpersonalised (which is based on data source type)?
 * 
 * Tradeoffs:
 * 1. Robustness vs Correctness (from Code Complete by Steve McConnell):
 * If a down stream service is broken, it's better to return cached data than to show an error message (ie. prioritise robustness over correctness).
 * 
 * 2. First Contentful Paint (FCP) and Total Load Time:
 * There are two options:
 *  a) One HTTP request to get all the shelves, but this takes longer to return data and means the user can't see anything above the fold.
 *  b) Two HTTP requests to get the critical (non-personalised) shelves first, then the personalised shelves.
 *     The overall load time is longer but at least the user sees something above the fold quickly.
 *  We will go with option a).
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
        public async Task<List<List<ShelfItem>>> GetShelves()
        {
            return await shelvesService.GetNonPersonalisedShelves();
        }
    }
}