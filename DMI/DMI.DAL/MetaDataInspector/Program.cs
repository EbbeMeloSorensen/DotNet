using MetaDataInspector;

const bool includeStationsFromDenmark = true;
const bool includeStationsFromGreenland = false;
const bool includeStationsFromFaroeIslands = false;
const bool includeStationsWithoutCountry = false;

var stations = DataHelpers.StatDB_RetrieveStations();

var stationInformations = DataHelpers.SMS_RetrieveStationInformations(
    includeSynopStations: false,
    includeSVKStations: false,
    includePluvioStations: true);

using var sw = new StreamWriter("output.txt");

foreach (var si in stationInformations)
{
    if ((si.CountryasString == "Danmark" && !includeStationsFromDenmark) ||
        (si.CountryasString == "Grønland" && !includeStationsFromGreenland) ||
        (si.CountryasString == "Færøerne" && !includeStationsFromFaroeIslands) ||
        (si.CountryasString == "<null>" && !includeStationsWithoutCountry))
    {
        continue;
    }

    sw.PrintLine($"{si.StationIDDMIAsString} ({si.StationNameAsString} - {si.StationTypeAsString})");
    sw.PrintLine("");

    // Identify the Station in statdb that matches the stationinformation
    var matchingStations = stations.Where(_ => _.statid / 100 == si.stationid_dmi);
    var matchingStationCount = matchingStations.Count();

    if (matchingStationCount == 0)
    {
        sw.PrintLine("NO MATCHING STATION IN STATDB");
        continue;
    }
    else if (matchingStationCount > 1)
    {
        sw.PrintLine("MULTIPLE MATCHING STATIONS IN STATDB");
        continue;
    }

    var s = matchingStations.Single();

    sw.PrintLine($"    icao id:                      {si.StationIDICAOAsString,40}");
    sw.InspectCountry(si, s, true);
    sw.PrintLine($"    station id:                   {si.StationIDDMIAsString,40}");
    sw.PrintLine($"    datefrom (sms):               {si.DateFromAsString,40}");
    sw.PrintLine($"    dateto (sms):                 {si.DateToAsString,40}");
    sw.PrintLine($"    station type:                 {si.StationTypeAsString,40}");
    sw.PrintLine($"    station owner:                {si.StationOwnerAsString,40}");

    // Fra det gamle script
    //PrintLine(sw, $"    source:                       {"",40} {sourceAsString,40}   ({sourceOK})");

    sw.PrintLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
}