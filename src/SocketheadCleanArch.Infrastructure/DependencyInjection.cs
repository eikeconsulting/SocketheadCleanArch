using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocketheadCleanArch.Admin.Data;
using SocketheadCleanArch.Domain.Contracts;

namespace SocketheadCleanArch.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Register infrastructure services
    /// </summary>
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddScoped<ISocketheadCleanArchDbContext, SocketheadCleanArchDbContext>()
            
            .AddDataProtection()
            .PersistKeysToDbContext<SocketheadCleanArchDbContext>()
            .Services
            
            ;
    }
}