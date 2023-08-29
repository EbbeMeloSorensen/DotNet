using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.API.Controllers;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API;

public class LocationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPeople([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
    }
}