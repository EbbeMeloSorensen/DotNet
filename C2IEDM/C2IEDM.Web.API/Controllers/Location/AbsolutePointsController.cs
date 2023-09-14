using Microsoft.AspNetCore.Mvc;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location;

public class AbsolutePointsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAbsolutePoints([FromQuery] PagingParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListAbsolutePoints.Query{ Params = param }));
    }

    [HttpGet("{timeOfInterest}")]
    public async Task<IActionResult> GetVerticalDistances_Historic([FromQuery] PagingParams param, DateTime timeOfInterest)
    {
        return HandlePagedResult(await Mediator.Send(new ListAbsolutePoints.Query { Params = param, TimeOfInterest = timeOfInterest }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditAbsolutePoint(Guid id, AbsolutePoint absolutePoint)
    {
        absolutePoint.Id = id;
        return HandleResult(await Mediator.Send(new EditAbsolutePoint.Command { AbsolutePoint = absolutePoint }));
    }
}