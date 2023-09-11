using C2IEDM.Web.Application.Locations;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers.Location;

public class RelativePointsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetRelativePoints([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Category = LocationCategory.RelativePoint,
            Params = param
        }));
    }
}