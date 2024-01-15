using WIGOS.Web.Application.Core;
using WIGOS.Web.Application.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace WIGOS.Web.API.Controllers.Location
{
    public class LinesController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetLines([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new ListLines.Query
            {
                Params = param
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLine(Guid id)
        {
            return HandleResult(await Mediator.Send(new LineDetails.Query { Id = id }));
        }
    }
}