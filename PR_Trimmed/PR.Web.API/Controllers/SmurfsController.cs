using Microsoft.AspNetCore.Mvc;
using PR.Web.Application.Smurfs;

namespace PR.Web.API.Controllers;

public class SmurfsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetSmurfs(
        [FromQuery] SmurfParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
    }
}