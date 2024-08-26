using System.Text;
using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class Status
{
    public static bool InspectStatus(
        this StreamWriter sw,
        StationInformation si,
        List<Domain.StatDB.Active> statuses,
        bool evaluate)
    {
        bool? status;
        var statusAsString = "";
        var statusMatches = false;
        var issue = "";

        var correspondingStatuses = statuses
            .Where(_ => _.statid / 100 == si.stationid_dmi)
            .ToList();

        if (!correspondingStatuses.Any())
        {
            issue = "NO STATUS FOUND IN STATDB";
        }
        else
        {
            status = correspondingStatuses
                .OrderBy(_ => _.start_time)
                .Last().active;

            if (status.HasValue)
            {
                statusAsString = status.Value ? "active" : "inactive";
            }

            statusMatches = si.StatusAsString == statusAsString;

            if (!statusMatches)
            {
                issue = "DIFFERS FROM SMS";
            }
        }

        var statusOK = statusMatches ? "ok" : $"INVALID ({issue})";

        var sb = new StringBuilder($"    status:                       {$"{si.StatusAsString}",40} {statusAsString,40}");

        if (evaluate)
        {
            sb.Append($"   ({statusOK})");
        }
 
        sw.PrintLine(sb.ToString());

        return statusMatches;
    }
}