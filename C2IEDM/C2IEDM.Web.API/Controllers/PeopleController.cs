using Microsoft.AspNetCore.Mvc;
using C2IEDM.Web.Application.People;

namespace C2IEDM.Web.API.Controllers;

public class PeopleController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPeople([FromQuery] PersonParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
    }
}