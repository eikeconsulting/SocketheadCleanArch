using Microsoft.AspNetCore.Identity;
using Serilog;
using SocketheadCleanArch.Admin;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Domain.Data.Entities;
using SocketheadCleanArch.Infrastructure;
using SocketheadCleanArch.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = Logging.CreateSerilogLogger(); 

builder.Host.UseSerilog();

builder.Services
    .AddHealthChecks()
    .Services
    .RegisterInfrastructure(builder.Configuration)
    .RegisterServices(builder.Configuration)
    .AddDatabaseDeveloperPageExceptionFilter()
    .AddDefaultIdentity<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    })
    .AddRoles<AppRole>()
    .AddEntityFrameworkStores<SocketheadCleanArchDbContext>()
    .AddDefaultTokenProviders()
    .Services
    //.AddDistributedMemoryCache() // Required for session
    //.AddSession() // Adds session services
    .AddControllersWithViews()
    //.AddSessionStateTempDataProvider()
    .Services

    .AddHsts(options => 
    {
        options.MaxAge = TimeSpan.FromDays(7);
        options.IncludeSubDomains = true;
    })
    
    .AddAuthorization(options =>
    {
        options.AddPolicy("RequireMFA", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("amr", "mfa"); // Require the "mfa" claim
        });
    })
    
    .AddScoped<DataSeeder>()

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

app.MapHealthChecks("/_health");

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