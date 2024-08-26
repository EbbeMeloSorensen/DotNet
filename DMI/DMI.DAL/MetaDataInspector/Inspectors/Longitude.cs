using System.Text;
using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class Longitude
{
    public static bool InspectLongitude(
        this StreamWriter sw,
        StationInformation si,
        List<Domain.StatDB.Position> positions,
        bool evaluate,
        double tolerance)
    {
        double? longitude;
        var longitudeAsString = "";
        var longitudeMatches = false;
        var issue = "";

        var correspondingPositions = positions
            .Where(_ => _.statid / 100 == si.stationid_dmi)
            .ToList();

        if (!correspondingPositions.Any())
        {
            issue = "NO POSITION FOUND IN STATDB";
        }
        else
        {
            longitude = correspondingPositions
                .OrderBy(_ => _.start_time)
                .Last().@long;

            longitudeAsString = longitude.HasValue ? $"{longitude.Value}" : "";
            var difference = 0.0;

            if (!si.wgs_long.HasValue)
            {
                longitudeMatches = !longitude.HasValue;
            }
            else if (!longitude.HasValue)
            {
                longitudeMatches = false;
            }
            else
            {
                difference = Math.Abs(si.wgs_long.Value - longitude.Value);

                longitudeMatches = difference <= tolerance;
            }

            if (!longitudeMatches)
            {
                issue = $"DIFFERS FROM SMS BY {difference:N6}";
            }
        }

        var longitudeOK = longitudeMatches ? "ok" : $"INVALID ({issue})";

        var sb = new StringBuilder($"    long:                         {$"{si.WGSLongAsString}",40} {$"{longitudeAsString}",40}");

        if (evaluate)
        {
            sb.Append($"   ({longitudeOK})");
        }
 
        sw.PrintLine(sb.ToString());

        return longitudeMatches;
    }
}