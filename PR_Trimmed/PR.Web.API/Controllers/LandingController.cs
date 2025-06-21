using Microsoft.AspNetCore.Mvc;

namespace PR.Web.API.Controllers;

// Bemærk, at den her overrider den, der kommer fra BaseApiController, som i øvrigt starter med api
[Route("api")]
public class LandingController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetLanding()
    {
        return HandleResult(await Mediator.Send(new Get.Query()));
    }
}