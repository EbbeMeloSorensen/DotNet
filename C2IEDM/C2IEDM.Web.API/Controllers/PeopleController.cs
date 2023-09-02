using C2IEDM.Web.Application.People;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers;

public class PeopleController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPeople([FromQuery] PersonParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
    }
}