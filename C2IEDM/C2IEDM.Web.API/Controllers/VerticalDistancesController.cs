using Microsoft.AspNetCore.Mvc;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Web.Application.Locations.VerticalDistance;

namespace C2IEDM.Web.API.Controllers;

public class VerticalDistancesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetVerticalDistances([FromQuery] Params param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateVerticalDistance(VerticalDistance verticalDistance)
    {
        return HandleResult(await Mediator.Send(new Create.Command { VerticalDistance = verticalDistance }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditVerticalDistance(Guid id, VerticalDistance verticalDistance)
    {
        verticalDistance.Id = id;
        return HandleResult(await Mediator.Send(new Edit.Command { VerticalDistance = verticalDistance }));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVerticalDistance(Guid id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }
}