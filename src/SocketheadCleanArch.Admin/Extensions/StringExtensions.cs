namespace SocketheadCleanArch.Admin.Extensions;

public static class StringExtensions
{
    public static string UppercaseFirst(this string s)
    {
        return string.IsNullOrEmpty(s)
            ? string.Empty
            : char.ToUpper(s[0]) + s[1..];
    }
}
