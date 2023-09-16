using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.WIGOS.GeospatialLocation;

namespace C2IEDM.Web.API.Controllers.WIGOS;

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