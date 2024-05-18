namespace MetaDataInspector;

public static class Helpers
{
    public static string AsDateString(
        this DateTime dateTime)
    {
        var year = dateTime.Year;
        var month = dateTime.Month.ToString().PadLeft(2, '0');
        var day = dateTime.Day.ToString().PadLeft(2, '0');

        var result = $"{year}-{month}-{day}";

        return result;
    }

    public static string AsDateTimeString(
        this DateTime dateTime,
        bool includeMilliseconds)
    {
        var year = dateTime.Year;
        var month = dateTime.Month.ToString().PadLeft(2, '0');
        var day = dateTime.Day.ToString().PadLeft(2, '0');

        var hour = dateTime.Hour.ToString().PadLeft(2, '0');
        var minute = dateTime.Minute.ToString().PadLeft(2, '0');
        var second = dateTime.Second.ToString().PadLeft(2, '0');

        var result = $"{year}-{month}-{day} {hour}:{minute}:{second}";

        if (includeMilliseconds)
        {
            var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

            result += $".{millisecond}";
        }

        return result;
    }

    public static void PrintLine(
        this StreamWriter streamWriter,
        string line)
    {
        Console.WriteLine(line);
        streamWriter.WriteLine(line);
    }
}