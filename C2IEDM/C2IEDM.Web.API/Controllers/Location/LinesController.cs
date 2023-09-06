using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location;

public class LinesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetLines([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Type = LocationType.Line,
            Params = param
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLine(Guid id)
    {
        return HandleResult(await Mediator.Send(new LineDetails.Query { Id = id }));
    }
}

public class PolygonAreasController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPolygonAreas([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query
        {
            Type = LocationType.PolygonArea,
            Params = param
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPolygonArea(Guid id)
    {
        return HandleResult(await Mediator.Send(new PolygonAreaDetails.Query { Id = id }));
    }
}