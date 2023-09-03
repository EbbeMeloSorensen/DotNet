using C2IEDM.Web.Application.Locations;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers;

public class AbsolutePointsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAbsolutePoints([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Type = LocationType.AbsolutePoint,
            Params = param
        }));
    }
}