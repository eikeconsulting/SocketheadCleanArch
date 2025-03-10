using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocketheadCleanArch.Service.Repository;

namespace SocketheadCleanArch.Service;

public static class DependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration config)
    {
        return services
                .AddScoped<SocketheadCleanArchRepository>()
                .AddScoped<UserAdminRepository>()
            ;
    }
}