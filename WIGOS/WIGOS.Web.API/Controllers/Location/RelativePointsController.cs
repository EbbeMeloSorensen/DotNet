using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.Location
{
    public class RelativePointsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetRelativePoints([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListLocations.Query
            {
                Category = LocationCategory.RelativePoint,
                Params = param
            }));
        }
    }
}