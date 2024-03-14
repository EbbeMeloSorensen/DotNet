using System.Text;

namespace MigrationScriptGenerator;

public static class Helpers
{
    public static void PrintLine(
        this StreamWriter streamWriter,
        string line,
        bool alsoWriteToConsole)
    {
        streamWriter.WriteLine(line);

        if (alsoWriteToConsole)
        {
            Console.WriteLine(line);
        }
    }

    public static DateTime AsDateTimeUTC(
        this string s)
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

    public static string AsDateTimeString(
        this DateTime dateTime)
    {
        var day = dateTime.Day;
        var month = dateTime.Month;
        var year = dateTime.Year;
        var hour = dateTime.Hour;
        var minute = dateTime.Minute;
        var second = dateTime.Second;
        var milliSecond = dateTime.Millisecond;

        if (year == 9999)
        {
            return $"                       ";
        }

        var sb = new StringBuilder();
        sb.Append($"{year}");
        sb.Append($"-{month.ToString().PadLeft(2, '0')}");
        sb.Append($"-{day.ToString().PadLeft(2, '0')}");
        sb.Append($" {hour.ToString().PadLeft(2, '0')}");
        sb.Append($":{minute.ToString().PadLeft(2, '0')}");
        sb.Append($":{second.ToString().PadLeft(2, '0')}");
        sb.Append($".{milliSecond.ToString().PadLeft(3, '0')}");

        var result = sb.ToString();

        return result;
    }

    public static bool Overlaps(
        this TimeInterval a,
        TimeInterval b)
    {
        var maxStartTime = a.Start > b.Start
            ? a.Start
            : b.Start;

        var minEndTime = a.End < b.End
            ? a.End
            : b.End;

        return maxStartTime < minEndTime;
    }

    public static bool CoveredBy(
        this TimeInterval a,
        TimeInterval b)
    {
        var temp1 = a.Start >= b.Start;
        var temp2 = a.End <= b.End;

        return temp1 && temp2;
    }

    public static TimeInterval Trim(
        this TimeInterval a,
        TimeInterval b)
    {
        if (a.Start >= b.Start && a.End < b.End)
        {
            throw new InvalidOperationException();
        }

        if (a.End <= b.Start || b.End <= a.Start)
        {
            return a;
        }

        if (a.Start < b.Start)
        {
            return new TimeInterval
            {
                Start = a.Start,
                End = b.Start
            };
        }
        else
        {
            return new TimeInterval
            {
                Start = b.End,
                End = a.End
            };
        }
    }
}