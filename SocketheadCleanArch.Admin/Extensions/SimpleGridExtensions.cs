using Sockethead.Razor.Css;
using Sockethead.Razor.Grid;

namespace SocketheadCleanArch.Admin.Extensions;

public static class SimpleGridExtensions
{
    public static SimpleGrid<T> AddButtonLinkColumn<T>(
        this SimpleGrid<T> grid, 
        string label, 
        Func<T, string> linkBuilder, 
        string? target = null,
        Action<CssBuilder>? css = null) 
        where T : class
    {
        return grid
            .AddColumn(col => col
                .Display(label)
                .LinkTo(
                    linkBuilder: linkBuilder,
                    target: target ?? "_self",
                    css: css ??= _css => _css.AddClass("btn btn-sm btn-primary")));
    }
    
    public static SimpleGrid<T> AddCustomPager<T>(
        this SimpleGrid<T> grid) where T : class
        => grid
            .AddPager(options =>
            {
                options.RowsPerPage = 20;
                options.DisplayPagerTop = true;
                options.DisplayPagerBottom = true;
                options.DisplayTotal = true;
                options.RowsPerPageOptions = [20, 50, 100, 1000];
            });
    
    public static SimpleGrid<T> AddCustomCss<T>(
        this SimpleGrid<T> grid) where T : class
        => grid
            .Css(elements =>
            {
                elements.Table.AddClass("table-striped table-sm table-bordered");
            });
    
}