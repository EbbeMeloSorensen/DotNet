using System.Text;
using MetaDataInspector.Domain.SMS;
using MetaDataInspector.Domain.StatDB;

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
        bool includeMilliseconds,
        bool omitTimeIfZero)
    {
        var year = dateTime.Year;
        var month = dateTime.Month.ToString().PadLeft(2, '0');
        var day = dateTime.Day.ToString().PadLeft(2, '0');

        var hour = dateTime.Hour.ToString().PadLeft(2, '0');
        var minute = dateTime.Minute.ToString().PadLeft(2, '0');
        var second = dateTime.Second.ToString().PadLeft(2, '0');
        var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

        var sb = new StringBuilder($"{year}-{month}-{day}");

        if (hour == "00" &&
            minute == "00" &&
            second == "00" &&
            millisecond == "000" &&
            omitTimeIfZero)
        {
            return sb.ToString();
        }

        sb.Append($" {hour}:{minute}:{second}");

        if (includeMilliseconds)
        {
            sb.Append($".{millisecond}");
        }

        return sb.ToString();
    }

    public static void PrintLine(
        this StreamWriter sw,
        string line)
    {
        Console.WriteLine(line);
        sw.WriteLine(line);
    }
}