namespace SocketheadCleanArch.Admin.Extensions;

public static class TimeSpanExtensions
{
    public static string ToPrettyFormat(this TimeSpan timeSpan)
    {
        string[] dayParts = new[]
            {
                GetDays(timeSpan), 
                GetHours(timeSpan), 
                GetMinutes(timeSpan)
            }
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();

        int numberOfParts = dayParts.Length;

        string result = numberOfParts switch
        {
            0 => "Less than a minute",
            1 => dayParts.First(),
            _ => string.Join(", ", dayParts, 0, numberOfParts - 1) + " and " + dayParts[numberOfParts - 1]
        };

        return result.UppercaseFirst();
    }    

    private static string GetMinutes(TimeSpan timeSpan)
    {
        return timeSpan.Minutes switch
        {
            0 => string.Empty,
            1 => "a minute",
            _ => timeSpan.Minutes + " minutes"
        };
    }

    private static string GetHours(TimeSpan timeSpan)
    {
        return timeSpan.Hours switch
        {
            0 => string.Empty,
            1 => "an hour",
            _ => timeSpan.Hours + " hours"
        };
    }

    private static string GetDays(TimeSpan timeSpan)
    {
        return timeSpan.Days switch
        {
            0 => string.Empty,
            1 => "a day",
            _ => timeSpan.Days + " days"
        };
    }
}