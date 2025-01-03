using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocketheadCleanArch.Admin.Extensions;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Service.Repository;

namespace SocketheadCleanArch.Admin.Controllers;

public class DeveloperController : Controller
{
    public IActionResult Dashboard()
    {
        this.SetTitle("Developer Dashboard");
        
        return View();
    }

    public async Task<IActionResult> MigrationsHistory([FromServices] SocketheadCleanArchRepository repo)
    {
        this.SetTitle(title: "Migrations History");

        IReadOnlyList<EFMigrationsHistory> history = await repo.GetMigrationsHistoryAsync();
        
        return View(history);
    }
    
    [HttpGet]
    [Authorize(Roles = "Developer")]
    public IActionResult Diagnostics([FromServices] IWebHostEnvironment  env)
    {
        this.SetTitle(title: "Diagnostics");

        DateTime buildTime = GetBuildTimeUtc();
        DateTime startTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
        
        Dictionary<string, string> model = new()
        {
            { "Environment", env.EnvironmentName },
            // ReSharper disable once HeuristicUnreachableCode
            { "Build Config", IsDebug ? "Debug" : "Release" },
            { "Debugger Attached", System.Diagnostics.Debugger.IsAttached.ToString() },
            { "Machine Name", System.Environment.MachineName },
            { "Host Name", System.Environment.GetEnvironmentVariable("HOST_HOSTNAME") ?? "(N/A)" },
            { "OS Version", System.Environment.OSVersion.VersionString },
            { "Culture", CultureInfo.CurrentCulture.DisplayName },
            { "Current Time", $"{DateTime.UtcNow.ToHumanReadable()} UTC" },
            { "Build Time", $"{buildTime.ToHumanReadable()} UTC ({(DateTime.UtcNow - buildTime).ToPrettyFormat()} ago)" },
            { "Start Time", $"{startTime.ToHumanReadable()} UTC ({(DateTime.UtcNow - startTime).ToPrettyFormat()} ago)" },
            { "ContentRootPath", env.ContentRootPath },
            { "WebRootPath", env.WebRootPath },
        };
        
        return View(model);
    }
    
    private const bool IsDebug = 
#if DEBUG
    true;
#else
    false;
#endif    
    
    private static DateTime GetBuildTimeUtc()
    {
        Assembly assembly = Assembly.GetEntryAssembly()!;
        return new FileInfo(assembly.Location).LastWriteTime.ToUniversalTime();
    }    
}