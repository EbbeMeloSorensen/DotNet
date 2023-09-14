using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Core;
using C2IEDM.Web.Application.Locations;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;

namespace C2IEDM.Web.API.Controllers.Location;

public class AbsolutePointsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAbsolutePoints([FromQuery] PagingParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListAbsolutePoints.Query
        {
            Params = param
        }));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditAbsolutePoint(Guid id, AbsolutePoint absolutePoint)
    {
        absolutePoint.Id = id;
        return HandleResult(await Mediator.Send(new EditAbsolutePoint.Command { AbsolutePoint = absolutePoint }));
    }
}