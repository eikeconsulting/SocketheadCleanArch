using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Converters;
using Scalar.AspNetCore;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.API.Authentication;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Infrastructure;
using SocketheadCleanArch.Service;
using SocketheadCleanArch.Service.Config;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ApplyDefaultLoggingConfig()
    .CreateLogger();

builder.Host.UseSerilog();
builder.BuildAppConfig(Log.Logger);


builder.Services

    // Add health endpoints automatically (won't be in Swagger)
    .AddHealthChecks()
    .Services

    .RegisterInfrastructure(builder.Configuration)
    .RegisterServices(builder.Configuration)
    .AddScoped<JwtTokenService>()
    .AddScoped<UserAuthService>()

    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<SocketheadCleanArchDbContext>()
    .AddDefaultTokenProviders()
    .Services
    
    // Data Protection token lifespan
    .Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(15))
    
    // JWT Authentication
    .RegisterJwtTokenAuthentication(builder.Configuration)

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
