using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SocketheadCleanArch.Admin.Extensions;

namespace SocketheadCleanArch.Service.Config;

public static class LoggingConfig
{
    public static LoggerConfiguration ApplyDefaultLoggingConfig(this LoggerConfiguration config)
    {
        return config

            // Filter out health checks from logs as they are too noisy and not useful
            .Filter.ByExcluding(logEvent =>
                logEvent.Properties.TryGetValue("RequestPath", out LogEventPropertyValue? path) &&
                path.ToString().Contains("/_health"))

            .MinimumLevel.Information()

            // Don't log ASP.NET Core stuff, it's too noisy
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    
            // In Development use pretty console logs, otherwise use JSON which can be picked up for analysis
            .If(condition: Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development",
        
                ifAction: _config => _config            
                    // turn off database query logging for now
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)

                    // pretty print to the console
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"),
        
                elseAction: _config => _config
                    // log Json to console and logs will get picked up properly by CloudWatch and Logstash
                    .WriteTo.Console(new CompactJsonFormatter())); 
    }
}