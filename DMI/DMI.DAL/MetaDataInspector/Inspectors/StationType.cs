using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class StationType
{
    public static void InspectStationType(
        this StreamWriter sw,
        StationInformation si)
    {
        sw.PrintLine($"    station type:                 {$"{si.StationTypeAsString}",40}");
    }
}