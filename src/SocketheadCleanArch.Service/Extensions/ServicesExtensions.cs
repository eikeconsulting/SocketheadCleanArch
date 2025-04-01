using Microsoft.Extensions.DependencyInjection;

namespace SocketheadCleanArch.Admin.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection If(
        this IServiceCollection services, 
        bool condition,
        Action<IServiceCollection> ifAction, 
        Action<IServiceCollection> elseAction)
    {
        if (condition)
            ifAction(services);
        else
            elseAction(services);
            
        return services;
    }
}