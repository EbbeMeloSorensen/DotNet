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

using var streamWriter = new StreamWriter("output.txt");

foreach (var si in stationInformations)
{
    if ((si.CountryasString == "Danmark" && !includeStationsFromDenmark) ||
        (si.CountryasString == "Grønland" && !includeStationsFromGreenland) ||
        (si.CountryasString == "Færøerne" && !includeStationsFromFaroeIslands) ||
        (si.CountryasString == "<null>" && !includeStationsWithoutCountry))
    {
        continue;
    }

    streamWriter.PrintLine($"{si.StationIDDMIAsString} ({si.StationNameAsString} - {si.StationTypeAsString})");
    streamWriter.PrintLine("");

    // Identify the Station in statdb that matches the stationinformation
    var matchingStations = stations.Where(_ => _.statid / 100 == si.stationid_dmi);
    var matchingStationCount = matchingStations.Count();

    if (matchingStationCount == 0)
    {
        streamWriter.PrintLine("NO MATCHING STATION IN STATDB");
        continue;
    }
    else if (matchingStationCount > 1)
    {
        streamWriter.PrintLine("MULTIPLE MATCHING STATIONS IN STATDB");
        continue;
    }

    var s = matchingStations.Single();

    // Dodo: Erstat med et kald til en funktion, der sammenligner icao_id
    streamWriter.PrintLine($"    country:                      {si.CountryasString,40} {s.CountryAsString, 40}");
    streamWriter.PrintLine($"    station id:                   {si.StationIDDMIAsString,40}");
    streamWriter.PrintLine($"    icao id:                      {si.StationIDICAOAsString,40}");
    streamWriter.PrintLine($"    datefrom (sms):               {si.DateFromAsString,40}");
    streamWriter.PrintLine($"    dateto (sms):                 {si.DateToAsString,40}");
    streamWriter.PrintLine($"    station type:                 {si.StationTypeAsString,40}");
    streamWriter.PrintLine($"    station owner:                {si.StationOwnerAsString,40}");

    // Fra det gamle script
    //PrintLine(streamWriter, $"    source:                       {"",40} {sourceAsString,40}   ({sourceOK})");

    streamWriter.PrintLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
}