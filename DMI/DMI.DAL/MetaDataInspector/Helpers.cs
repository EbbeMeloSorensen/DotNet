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
        this StreamWriter sw,
        string line)
    {
        Console.WriteLine(line);
        sw.WriteLine(line);
    }

    public static void InspectCountry(
        this StreamWriter sw,
        StationInformation si,
        Station s,
        bool evaluate)
    {
        var countryMatches = false;

        if (!si.country.HasValue)
        {
            countryMatches = s.country == null;
        }
        else if (s.country == null)
        {
            countryMatches = false;
        }
        else
        {
            countryMatches = s.country == si.CountryasString;
        }

        var countryOK = countryMatches ? "ok" : "INVALID";

        var sb = new StringBuilder($"    country:                      {si.CountryasString,40} {s.CountryAsString,40}");

        if (evaluate)
        {
            sb.Append($"   ({countryOK})");
        }

        sw.PrintLine(sb.ToString());
    }

    public static void InspectIcaoID(
        this StreamWriter sw,
        StationInformation si,
        Station s,
        bool evaluate)
    {
        var icaoIDMatches = false;

        if (string.IsNullOrEmpty(si.stationid_icao))
        {
            icaoIDMatches = string.IsNullOrEmpty(s.icao_id);
        }
        else if (string.IsNullOrEmpty(s.icao_id))
        {
            icaoIDMatches = false;
        }
        else
        {
            icaoIDMatches = s.icao_id == si.stationid_icao;
        }

        var icaoIDOK = icaoIDMatches ? "ok" : "INVALID (DIFFERS FROM SMS)";

        var sb = new StringBuilder($"    icao id:                      {si.CountryasString,40} {s.CountryAsString,40}");

        if (evaluate)
        {
            sb.Append($"   ({icaoIDOK})");
        }

        sw.PrintLine(sb.ToString());
    }

}