using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Domain.Contracts;

namespace SocketheadCleanArch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        string connectionString = config.GetConnectionString("DefaultConnection") ??
                                  throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        return services
            .AddDbContext<SocketheadCleanArchDbContext>(options =>
            {
                options.UseSqlite(connectionString); // Change this for your project!
                options.EnableDetailedErrors();
            })
            .AddScoped<ISocketheadCleanArchDbContext, SocketheadCleanArchDbContext>()
            ;
    }
}