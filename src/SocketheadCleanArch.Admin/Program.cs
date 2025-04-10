using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Admin.Extensions;
using SocketheadCleanArch.Admin.MFA;
using SocketheadCleanArch.Admin.Seeding;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Infrastructure;
using SocketheadCleanArch.Infrastructure.Postgres;
using SocketheadCleanArch.Infrastructure.Sqlite;
using SocketheadCleanArch.Service;
using SocketheadCleanArch.Service.Config;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ApplyDefaultLoggingConfig()
    .CreateLogger();

builder.Host.UseSerilog();
builder.BuildAppConfig(Log.Logger);

IConfiguration config = builder.Configuration;

builder.Services
    .AddHealthChecks()
    .Services
    
    // Register the database provider based on configuration
    .If(config["DatabaseProvider"] == "Postgres",
        ifAction: sp => sp.RegisterPostgres(config),
        elseAction: sp => sp.RegisterSqlite(config))
    
    .RegisterInfrastructure(config)
    .RegisterServices(config)

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
    
    // Setup MFA, this is not working, it never calls this
    // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/mfa?view=aspnetcore-9.0
    .AddScoped<IUserClaimsPrincipalFactory<AppUser>, AdditionalUserClaimsPrincipalFactory>()
    
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
    .Configure<DataSeedSettings>(config.GetSection("DataSeedSettings"))
    .AddScoped<DataSeeder>()
    .AddRazorPages()
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
    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseExceptionHandler("/Home/Error");
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();

// automatically run the migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SocketheadCleanArchDbContext>();
    db.Database.Migrate();
}

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

await app.RunAsync();