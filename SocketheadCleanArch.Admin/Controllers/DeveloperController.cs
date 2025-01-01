using Microsoft.AspNetCore.Mvc;

namespace SocketheadCleanArch.Admin.Controllers;

public class DeveloperController(
    //ILogger<DeveloperController> logger
    ) : Controller
{
    public IActionResult Dashboard()
    {
        return Content("Coming soon");
    }

}