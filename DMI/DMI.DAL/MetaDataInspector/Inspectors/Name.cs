using System.Text;
using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class Name
{
    public static bool InspectName(
        this StreamWriter sw,
        StationInformation si,
        List<Domain.StatDB.Name> names,
        bool evaluate)
    {
        var name = "";
        var nameMatches = false;
        var issue = "";

        var correspondingNames = names
            .Where(_ => _.statid / 100 == si.stationid_dmi)
            .ToList();

        if (!correspondingNames.Any())
        {
            issue = "NO NAME FOUND IN STATDB";
        }
        else
        {
            name = correspondingNames
                .OrderBy(_ => _.start_time)
                .Last().name;

            var nameAsString = string.IsNullOrEmpty(name) ? "" : name;
            nameMatches = si.StationNameAsString.ToUpper() == nameAsString.ToUpper();

            if (!nameMatches)
            {
                issue = "DIFFERS FROM SMS";
            }
        }

        var nameOK = nameMatches ? "ok" : $"INVALID ({issue})";

        var sb = new StringBuilder($"    name:                         {$"{si.StationNameAsString}",40} {name,40}");

        if (evaluate)
        {
            sb.Append($"   ({nameOK})");
        }
 
        sw.PrintLine(sb.ToString());

        return nameMatches;
    }
}