using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class StationOwner
{
    public static void InspectStationOwner(
        this StreamWriter sw,
        StationInformation si)
    {
        sw.PrintLine($"    station owner:                {$"{si.StationOwnerAsString}",40}");
    }
}