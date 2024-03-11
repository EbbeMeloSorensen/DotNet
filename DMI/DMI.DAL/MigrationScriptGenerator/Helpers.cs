namespace MigrationScriptGenerator;

public static class Helpers
{
    public static DateTime AsDateTimeUTC(this string s)
    {
        if (s.Contains("infinity"))
        {
            return DateTime.MaxValue;
        }

        var year = int.Parse(s.Substring(0, 4));
        var month = int.Parse(s.Substring(5, 2));
        var day = int.Parse(s.Substring(8, 2));
        var hour = int.Parse(s.Substring(11, 2));
        var minute = int.Parse(s.Substring(14, 2));
        var second = int.Parse(s.Substring(17, 2));
        var milliSecond = s.Length <= 20 ? 0 : int.Parse(s.Substring(20));

        return new DateTime(year, month, day, hour, minute, second, milliSecond, DateTimeKind.Utc);
    }

    public static string AsShortDateString(this DateTime dateTime)
    {
        var day = dateTime.Day;
        var month = dateTime.Month;
        var year = dateTime.Year;

        return $"{day}-{month}-{year}";
    }
}