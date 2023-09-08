using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers;

public class VerticalDistancesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetVerticalDistances([FromQuery] VerticalDistanceParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListVerticalDistances.Query { Params = param }));
    }
}