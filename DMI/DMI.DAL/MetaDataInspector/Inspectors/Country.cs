using System.Text;
using MetaDataInspector.Domain.SMS;
using MetaDataInspector.Domain.StatDB;

namespace MetaDataInspector.Inspectors;

public static class Country
{
    public static bool InspectCountry(
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

        return countryMatches;
    }
}