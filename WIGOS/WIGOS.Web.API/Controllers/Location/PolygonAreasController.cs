using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.Location
{
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
}