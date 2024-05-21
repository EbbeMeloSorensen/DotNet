using MetaDataInspector.Domain.SMS;

namespace MetaDataInspector.Inspectors;

public static class DateFrom
{
    public static void InspectDateFrom(
        this StreamWriter sw,
        StationInformation si)
    {
        sw.PrintLine($"    datefrom (sms):               {$"{si.DateFromAsString}",40}");
    }
}