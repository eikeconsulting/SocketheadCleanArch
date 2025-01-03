using System.Globalization;

namespace SocketheadCleanArch.Admin.Extensions;

public static class DateTimeExtensions
{
    public static string ToHumanReadable(this DateTime date)
    {
        return date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
}