using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.Location
{
    public class AbsolutePointsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAbsolutePoints([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListAbsolutePoints.Query { Params = param }));
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
}