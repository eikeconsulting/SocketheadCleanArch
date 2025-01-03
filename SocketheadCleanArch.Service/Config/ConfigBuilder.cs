using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SocketheadCleanArch.Service.Config;

public static class ConfigBuilder
{
    /// <summary>
    /// Build the Configuration from appsettings.json files loaded in the following order:
    /// 1. Shared appsettings.json
    /// 2. Shared appsettings.[environment].json
    /// 3. Machine specific shared file appsettings.[machine].json
    /// 4. Local Project appsettings.json
    /// 5. Local Project appsettings.[environment].json
    /// The shared files should be placed at the root of the Solution
    /// </summary>
    /// <param name="builder">This Config Builder object</param>
    /// <param name="logger">Serilog Logger (ILogger can't be injected)</param>
    /// <returns>Builder</returns>
    public static IConfigurationBuilder BuildAppConfig(this IHostApplicationBuilder builder, ILogger logger)
    {
        string configPath = Path.Combine(builder.Environment.ContentRootPath, "..");
        string environment = builder.Environment.EnvironmentName;
        string machine = Environment.MachineName;
        
        logger.Information("Building App Config: [{ConfigPath}] [{Environment}] [{Machine}]", configPath, environment, machine);
        
        return builder
            .Configuration
            .AddJsonFile(Path.Combine(configPath, "appsettings.json"), optional: true, reloadOnChange: true)
            .AddJsonFile(Path.Combine(configPath, $"appsettings.{environment}.json"), optional: true, reloadOnChange: true)
            .AddJsonFile(Path.Combine(configPath, $"appsettings.{machine}.json"), optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}