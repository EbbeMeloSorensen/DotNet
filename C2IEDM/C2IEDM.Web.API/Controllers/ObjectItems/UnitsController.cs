using C2IEDM.Web.Application.ObjectItems;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers.ObjectItems;

public class UnitsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetUnits([FromQuery] ObjectItemParams param)
    {
        return HandlePagedResult(await Mediator.Send(new ListObjectItems.Query
        {
            Category = ObjectItemCategory.Unit,
            Params = param
        }));
    }
}