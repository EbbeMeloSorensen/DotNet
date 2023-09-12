using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location;

public class SurfacesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetLocations([FromQuery] PagingParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Category = LocationCategory.Surface,
            Params = param
        }));
    }
}