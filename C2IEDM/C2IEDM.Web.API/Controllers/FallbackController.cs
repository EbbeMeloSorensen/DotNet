using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C2IEDM.Web.API.Controllers;

[AllowAnonymous]
public class FallbackController : Controller
{
    public IActionResult Index()
    {
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");
    }
}
