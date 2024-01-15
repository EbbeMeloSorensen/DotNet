using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.WIGOS.GeospatialLocation;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.WIGOS
{
    public class GeospatialLocationsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetGeospatialLocations([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query
            {
                Category = GeospatialLocationCategory.GeospatialLocation,
                Params = param
            }));
        }
    }
}