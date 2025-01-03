using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocketheadCleanArch.Admin.Data;

namespace SocketheadCleanArch.Tests;

public class ApiAppFactory : WebApplicationFactory<ApiProgram>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        
        base.ConfigureWebHost(builder);

        // override here
        builder.ConfigureServices(services =>
        {
            services
                // TODO: remove this once using a real database
                .AddDbContext<SocketheadCleanArchDbContext>(options =>
                {
                    options.UseSqlite("DataSource=../../../../app.db;Cache=Shared");
                })
                ;
        });
    }
}