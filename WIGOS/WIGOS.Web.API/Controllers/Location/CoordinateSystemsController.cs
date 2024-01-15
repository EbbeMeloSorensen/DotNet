using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.Location
{
    public class CoordinateSystemsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCoordinateSystems([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListCoordinateSystems.Query
            {
                Type = CoordinateSystemType.CoordinateSystem,
                Params = param
            }));
        }
    }
}
