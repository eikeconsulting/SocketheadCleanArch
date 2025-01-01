using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SocketheadCleanArch.Admin.Extensions;

public static class HtmlHelperExtensions
{
    /// <summary>
    /// This will set the H1 tag of the page as well as title in the HTML metadata
    /// </summary>
    public static void SetTitle(this IHtmlHelper htmlHelper, string title, bool hide = false)
    {
        htmlHelper.ViewData["Title"] = title;
        htmlHelper.ViewData["HideTitle"] = hide;
    }
    
    /// <summary>
    /// Generate a dropdown control for an enumeration
    /// It is hard to understand why this is not part of Microsoft's library
    /// </summary>
    public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TEnum>> expression)
    {
        return htmlHelper.DropDownListFor(
            expression: expression,
            Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e!.ToString(),
                    Text = e.ToString()
                }),
            new { @class = "form-control" });
    }
}