using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Domain.Contracts;

namespace SocketheadCleanArch.Infrastructure.Postgres;

public static class DependencyInjection
{
    private const string Provider = "Postgres";
    
    public static IServiceCollection RegisterPostgres(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddDbContext<SocketheadCleanArchDbContext>(options =>
            {
                string connectionString = config.GetConnectionString(Provider) ??
                                          throw new InvalidOperationException(
                                              $"Connection string '{Provider}' not found.");

                options.UseNpgsql(connectionString, x => x.MigrationsAssembly(typeof(DependencyInjection).Assembly.GetName().Name));
                options.EnableDetailedErrors();
            });
    }
}