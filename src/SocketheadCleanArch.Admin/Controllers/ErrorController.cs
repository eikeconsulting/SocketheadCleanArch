using Microsoft.AspNetCore.Mvc;

namespace SocketheadCleanArch.Admin.Controllers;


public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        if (statusCode == 404)
        {
            return View("NotFound");
        }
        
        return View("Error"); // Default error view for other status codes
    }
}