using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class Name
{
    public static void InspectName(
        this StreamWriter sw,
        StationInformation si)
    {
        sw.PrintLine($"    name:                         {$"{si.StationNameAsString}",40}");
    }
}

public static class Status
{
    public static void InspectStatus(
        this StreamWriter sw,
        StationInformation si)
    {
        sw.PrintLine($"    status:                       {$"{si.StatusAsString}",40}");
    }
}