using SocketheadCleanArch.Admin.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SocketheadCleanArch.Admin;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Domain.Data.Entities;
using SocketheadCleanArch.Infrastructure;
using SocketheadCleanArch.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()

    // Filter out health checks from logs as they are too noisy and not useful
    .Filter.ByExcluding(logEvent =>
        logEvent.Properties.TryGetValue("RequestPath", out var path) &&
        path.ToString().Contains("/_health"))

    .MinimumLevel.Information()

    // Don't log ASP.NET Core stuff, it's too noisy
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    
    // In Development use pretty console logs, otherwise use JSON which can be picked up for analysis
    .If(condition: Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development",
        
        ifAction: config => config            
            // turn off database query logging for now
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)

            // pretty print to the console
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"),
        
        elseAction: config => config
            // log Json to console and logs will get picked up properly by CloudWatch and Logstash
            .WriteTo.Console(new CompactJsonFormatter())) 
    
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services
    .RegisterInfrastructure(builder.Configuration)
    .RegisterServices(builder.Configuration)
    .AddScoped<DataSeeder>()
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddDefaultIdentity<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<AppRole>()
    .AddEntityFrameworkStores<SocketheadCleanArchDbContext>()
    .Services

        
    //.AddDistributedMemoryCache() // Required for session
    //.AddSession() // Adds session services
    .AddControllersWithViews()
    //.AddSessionStateTempDataProvider()
    
    ;

WebApplication app = builder.Build();


// We are using Serilog for request logging instead of built-in middleware
// https://github.com/serilog/serilog-aspnetcore?tab=readme-ov-file
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// this is required for Sockethead.Grid
app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

await app.SeedDataAsync();

app.Run();