using WIGOS.Domain.Entities.Geometry;
using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry.VerticalDistance;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers
{
    public class VerticalDistancesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetVerticalDistances([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{timeOfInterest}")]
        public async Task<IActionResult> GetVerticalDistances_Historic([FromQuery] PagingParams param, DateTime timeOfInterest)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param, TimeOfInterest = timeOfInterest }));
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
}