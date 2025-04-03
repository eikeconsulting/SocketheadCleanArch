using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Converters;
using Scalar.AspNetCore;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Admin.Extensions;
using SocketheadCleanArch.API.Authentication;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Infrastructure;
using SocketheadCleanArch.Infrastructure.Postgres;
using SocketheadCleanArch.Infrastructure.Sqlite;
using SocketheadCleanArch.Service;
using SocketheadCleanArch.Service.Config;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ApplyDefaultLoggingConfig()
    .CreateLogger();

builder.Host.UseSerilog();
builder.BuildAppConfig(Log.Logger);

IConfiguration config = builder.Configuration;

builder.Services

    // Add health endpoints automatically (won't be in Swagger)
    .AddHealthChecks()
    .Services

    // Register the database provider based on configuration
    .If(config["DatabaseProvider"] == "Postgres",
        ifAction: sp => sp.RegisterPostgres(config),
        elseAction: sp => sp.RegisterSqlite(config))
        
    .RegisterInfrastructure(config)
    .RegisterServices(config)
    .AddScoped<JwtTokenService>()
    .AddScoped<UserAuthService>()

    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<SocketheadCleanArchDbContext>()
    .AddDefaultTokenProviders()
    .Services
    
    // Data Protection token lifespan
    .Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(15))
    
    // JWT Authentication
    .RegisterJwtTokenAuthentication(config)
    
    // Deal with unhandled Exceptions and create a structured ProblemDetails response
    //.AddExceptionHandler<ApiExceptionHandler>()

    // Standardized Problem Details response for errors (both 400 and 500)
    .AddProblemDetails(options =>
        options.CustomizeProblemDetails = ctx =>
        {
            HttpRequest request = ctx.HttpContext.Request;
            ctx.ProblemDetails.Instance = $"{request.Method} {request.Scheme}://{request.Host}{request.Path}";
        })

    // Automatically validate input models via FluentValidation
    .AddFluentValidationAutoValidation()
    
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    })
    .Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddGoogle(options => {
        options.ClientId = config["Authentication:Google:ClientId"] ??"";
        options.ClientSecret = config["Authentication:Google:ClientSecret"] ??"";
    })
    .Services
    .AddHttpContextAccessor()
    .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    .AddOpenApi(
        documentName: "v1",  
        configureOptions: options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "SocketheadCleanArch API";
        options.Theme = ScalarTheme.BluePlanet;
        options.DarkMode = false;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.ShowSidebar = true;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public class ApiProgram;
