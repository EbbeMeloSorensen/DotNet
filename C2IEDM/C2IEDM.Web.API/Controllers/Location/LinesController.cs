using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location;

public class LinesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetLines([FromQuery] LocationParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListLocations.Query
        {
            Category = LocationCategory.Line,
            Params = param
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLine(Guid id)
    {
        return HandleResult(await Mediator.Send(new LineDetails.Query { Id = id }));
    }
}