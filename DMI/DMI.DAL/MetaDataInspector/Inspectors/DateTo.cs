using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class DateTo
{
    public static void InspectDateTo(
        this StreamWriter sw,
        StationInformation si)
    {
        sw.PrintLine($"    dateto   (sms):               {$"{si.DateToAsString}",40}");
    }
}