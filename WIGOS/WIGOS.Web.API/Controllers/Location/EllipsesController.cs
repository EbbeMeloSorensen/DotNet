using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.Location
{
    public class EllipsesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetEllipses([FromQuery] PagingParams param)
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
}