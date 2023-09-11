using C2IEDM.Web.Application.Locations;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers.Location;

public class EllipsesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetEllipses([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Category = LocationCategory.Ellipse,
            Params = param
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEllipses(Guid id)
    {
        return HandleResult(await Mediator.Send(new EllipseDetails.Query { Id = id }));
    }
}