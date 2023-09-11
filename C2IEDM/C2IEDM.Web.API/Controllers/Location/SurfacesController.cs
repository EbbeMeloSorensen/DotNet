using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location;

public class SurfacesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetLocations([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Category = LocationCategory.Surface,
            Params = param
        }));
    }
}