namespace CryBits.Host.Hosting;

using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Providers;

public static class LoggingConfiguration
{
    public static ILoggingBuilder AddCryBitsLogging(this ILoggingBuilder builder,
        Action<ZLoggerOptions>? configureOptions = null, bool enableRollingFile = false)
    {
        builder.ClearProviders();

        void ConfigureOptions(ZLoggerOptions options)
        {
            options.UsePlainTextFormatter(formatter =>
            {
                formatter.SetPrefixFormatter($"{0:local-longdate} [{1:short}]",
                    (in template, in info) => template.Format(info.Timestamp, info.LogLevel));
            });
            configureOptions?.Invoke(options);
        }

        builder.AddZLoggerConsole(ConfigureOptions);

        if (enableRollingFile)
        {
            builder.AddZLoggerRollingFile(
                (dt, index) => $"logs/crybits-{dt:yyyy-MM-dd}_{index}.log",
                RollingInterval.Day);
        }

        return builder;
    }
}
