using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.Locations;

namespace C2IEDM.Web.API.Controllers.Location
{
    public class CoordinateSystemsController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCoordinateSystems([FromQuery] CoordinateSystemParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListCoordinateSystems.Query
            {
                Type = CoordinateSystemType.CoordinateSystem,
                Params = param
            }));
        }
    }
}
