using Serilog;

namespace SocketheadCleanArch.Admin.Extensions;

public static class SerilogExtensions
{
    public static LoggerConfiguration If(
        this LoggerConfiguration loggerConfiguration,
        bool condition,
        Action<LoggerConfiguration> ifAction,
        Action<LoggerConfiguration>? elseAction = null)
    {
        if (condition)
            ifAction(loggerConfiguration);
        else
            elseAction?.Invoke(loggerConfiguration);
        
        return loggerConfiguration;
    }
}