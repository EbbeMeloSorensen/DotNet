using MetaDataInspector.Domain.SMS;
using MetaDataInspector.Domain.StatDB;

namespace MetaDataInspector.Inspectors;

public static class StationID
{
    public static void InspectStationID(
        this StreamWriter sw,
        StationInformation si,
        Station s)
    {
        sw.PrintLine($"    station id:                   {si.StationIDDMIAsString,40} {s.StatIDAsString,40}");
    }
}