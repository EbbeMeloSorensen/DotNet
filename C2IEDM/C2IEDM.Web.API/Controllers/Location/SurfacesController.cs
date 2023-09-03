using C2IEDM.Web.Application.Locations;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers.Location;

public class SurfacesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetLocations([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Type = LocationType.Surface,
            Params = param
        }));
    }
}