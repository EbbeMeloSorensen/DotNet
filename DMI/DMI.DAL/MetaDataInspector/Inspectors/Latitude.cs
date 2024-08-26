using System.Text;
using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class Latitude
{
    public static bool InspectLatitude(
        this StreamWriter sw,
        StationInformation si,
        List<Domain.StatDB.Position> positions,
        bool evaluate,
        double tolerance)
    {
        double? latitude;
        var latitudeAsString = "";
        var latitudeMatches = false;
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
            latitude = correspondingPositions
                .OrderBy(_ => _.start_time)
                .Last().lat;

            latitudeAsString = latitude.HasValue ? $"{latitude.Value}" : "";
            var difference = 0.0;

            if (!si.wgs_lat.HasValue)
            {
                latitudeMatches = !latitude.HasValue;
            }
            else if (!latitude.HasValue)
            {
                latitudeMatches = false;
            }
            else
            {
                difference = Math.Abs(si.wgs_lat.Value - latitude.Value);

                latitudeMatches = difference <= tolerance;
            }

            if (!latitudeMatches)
            {
                issue = $"DIFFERS FROM SMS BY {difference:N6}";
            }
        }

        var latitudeOK = latitudeMatches ? "ok" : $"INVALID ({issue})";

        var sb = new StringBuilder($"    lat:                          {$"{si.WGSLatAsString}",40} {$"{latitudeAsString}",40}");

        if (evaluate)
        {
            sb.Append($"   ({latitudeOK})");
        }
 
        sw.PrintLine(sb.ToString());

        return latitudeMatches;
    }
}