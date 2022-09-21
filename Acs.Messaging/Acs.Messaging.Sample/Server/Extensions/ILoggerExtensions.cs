using System.Runtime.CompilerServices;

namespace Acs.Messaging.Sample.Server.Extensions;

public static class ILoggerExtensions
{
    private const string prefix = "out: ";

    public static void Info(this ILogger logger, string value, [CallerMemberName] string? caller = null)
    {
        logger.LogInformation("{Prefix}{Caller}|{Value}", prefix, caller, value);
    }

    public static void Warn(this ILogger logger, string value, [CallerMemberName] string? caller = null)
    {
        logger.LogWarning("{Prefix}{Caller}|{Value}", prefix, caller, value);
    }

    public static void Error(this ILogger logger, Exception exception, string? value = null, [CallerMemberName] string? caller = null)
    {
        logger.LogWarning("{Prefix}{Caller}|{Value}{Exception}", prefix, caller, (string.IsNullOrEmpty(value) ? "" : $"{value}, "), exception);
    }
}