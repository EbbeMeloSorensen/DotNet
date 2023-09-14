using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Geometry;

namespace C2IEDM.Web.API.Controllers.Location;

public class RelativePointsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetRelativePoints([FromQuery] PagingParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Category = LocationCategory.RelativePoint,
            Params = param
        }));
    }
}