using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.Location
{
    public class SurfacesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetLocations([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListLocations.Query
            {
                Category = LocationCategory.Surface,
                Params = param
            }));
        }
    }
}