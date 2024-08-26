using System.Text;
using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class Height
{
    public static bool InspectHeight(
        this StreamWriter sw,
        StationInformation si,
        List<Domain.StatDB.Position> positions,
        bool evaluate,
        double tolerance)
    {
        double? height;
        var heightAsString = "";
        var heightMatches = false;
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
            height = correspondingPositions
                .OrderBy(_ => _.start_time)
                .Last().@height;

            heightAsString = height.HasValue ? $"{height.Value}" : "";
            var difference = 0.0;

            if (!si.hha.HasValue)
            {
                heightMatches = !height.HasValue;
            }
            else if (!height.HasValue)
            {
                heightMatches = false;
            }
            else
            {
                difference = Math.Abs(si.hha.Value - height.Value);

                heightMatches = difference <= tolerance;
            }

            if (!heightMatches)
            {
                if (difference > double.Epsilon)
                {
                    issue = $"DIFFERS FROM SMS BY {difference:N6}";
                }
                else
                {
                    issue = $"DIFFERS FROM SMS";
                }
            }
        }

        var heightOK = heightMatches ? "ok" : $"INVALID ({issue})";

        var sb = new StringBuilder($"    long:                         {$"{si.HHAAsString}",40} {$"{heightAsString}",40}");

        if (evaluate)
        {
            sb.Append($"   ({heightOK})");
        }
 
        sw.PrintLine(sb.ToString());

        return heightMatches;
    }
}