using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location;

public class PolygonAreasController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPolygonAreas([FromQuery] PagingParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Category = LocationCategory.PolygonArea,
            Params = param
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPolygonArea(Guid id)
    {
        return HandleResult(await Mediator.Send(new PolygonAreaDetails.Query { Id = id }));
    }
}