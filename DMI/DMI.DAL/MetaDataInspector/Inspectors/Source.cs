using MetaDataInspector.Domain.StatDB;

namespace MetaDataInspector.Inspectors;

public static class Source
{
    public static void InspectSource(
        this StreamWriter sw,
        Station s)
    {
        sw.PrintLine($"    source:                       {"",40} {s.SourceAsString,40}");
    }
}