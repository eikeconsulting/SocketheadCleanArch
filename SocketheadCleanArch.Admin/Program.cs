using Microsoft.AspNetCore.Identity;
using Serilog;
using SocketheadCleanArch.Admin;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Admin.MFA;
using SocketheadCleanArch.Admin.Seeding;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Infrastructure;
using SocketheadCleanArch.Service;
using SocketheadCleanArch.Service.Config;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'SocketheadCleanArchDbContextConnection' not found.");;

builder.Services.AddDbContext<SocketheadCleanArchDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
})
    .AddRoles<AppRole>()
    .AddEntityFrameworkStores<SocketheadCleanArchDbContext>()
    .AddDefaultTokenProviders();

Log.Logger = new LoggerConfiguration()
    .ApplyDefaultLoggingConfig()
    .CreateLogger();

builder.Host.UseSerilog();
builder.BuildAppConfig(Log.Logger);

builder.Services
    .AddHealthChecks()
    .Services
    .RegisterInfrastructure(builder.Configuration)
    .RegisterServices(builder.Configuration)
    .AddDatabaseDeveloperPageExceptionFilter()
    //.AddIdentity<AppUser, AppRole>(options =>
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
    .Configure<DataSeedSettings>(builder.Configuration.GetSection("DataSeedSettings"))
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
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/Home/Error");
app.UseDeveloperExceptionPage();
app.UseStatusCodePagesWithReExecute("/Error/{0}");

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