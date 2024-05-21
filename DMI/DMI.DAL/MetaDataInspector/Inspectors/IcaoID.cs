using System.Text;
using MetaDataInspector.Domain.SMS;
using MetaDataInspector.Domain.StatDB;

namespace MetaDataInspector.Inspectors;

public static class IcaoID
{
    public static void InspectIcaoID(
        this StreamWriter sw,
        StationInformation si,
        Station s,
        bool evaluate)
    {
        var icaoIDMatches = false;

        if (string.IsNullOrEmpty(si.stationid_icao))
        {
            icaoIDMatches = string.IsNullOrEmpty(s.icao_id);
        }
        else if (string.IsNullOrEmpty(s.icao_id))
        {
            icaoIDMatches = false;
        }
        else
        {
            icaoIDMatches = s.icao_id == si.stationid_icao;
        }

        var icaoIDOK = icaoIDMatches ? "ok" : "INVALID (DIFFERS FROM SMS)";

        var sb = new StringBuilder($"    icao id:                      {si.StationIDICAOAsString,40} {s.IcaoIDAsString,40}");

        if (evaluate)
        {
            sb.Append($"   ({icaoIDOK})");
        }

        sw.PrintLine(sb.ToString());
    }
}