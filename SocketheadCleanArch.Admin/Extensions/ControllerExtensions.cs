using Microsoft.AspNetCore.Mvc;

namespace ReaderBee.Admin.Extensions;

public static class ControllerExtensions
{
    public static void SetTitle(this Controller controller, string title, bool hide = false)
    {
        controller.ViewData["Title"] = title;
        controller.ViewData["HideTitle"] = hide;
    }
    
    public static IActionResult RedirectToReferer(this Controller controller) => 
        controller.Redirect(controller.Request.Headers.Referer.ToString());
    
}