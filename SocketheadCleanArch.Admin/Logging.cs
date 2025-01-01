using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SocketheadCleanArch.Admin.Extensions;

namespace SocketheadCleanArch.Admin;

public static class Logging
{
    public static Logger CreateSerilogLogger()
    {
        return new LoggerConfiguration()

            // Filter out health checks from logs as they are too noisy and not useful
            .Filter.ByExcluding(logEvent =>
                logEvent.Properties.TryGetValue("RequestPath", out var path) &&
                path.ToString().Contains("/_health"))

            .MinimumLevel.Information()

            // Don't log ASP.NET Core stuff, it's too noisy
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    
            // In Development use pretty console logs, otherwise use JSON which can be picked up for analysis
            .If(condition: Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development",
        
                ifAction: config => config            
                    // turn off database query logging for now
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)

                    // pretty print to the console
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"),
        
                elseAction: config => config
                    // log Json to console and logs will get picked up properly by CloudWatch and Logstash
                    .WriteTo.Console(new CompactJsonFormatter())) 
    
            .CreateLogger();
    }
}