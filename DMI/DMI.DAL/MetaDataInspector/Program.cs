using MetaDataInspector;
using MetaDataInspector.Inspectors;

const bool includeStationsFromDenmark = true;
const bool includeStationsFromGreenland = false;
const bool includeStationsFromFaroeIslands = false;
const bool includeStationsWithoutCountry = false;

var stations = StatDB.RetrieveStations();

var stationInformations = SMS.RetrieveStationInformations(
    includeSynopStations: true,
    includeSVKStations: true,
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
    }
    else if (matchingStationCount > 1)
    {
        sw.PrintLine("MULTIPLE MATCHING STATIONS IN STATDB");
    }
    else
    {
        var s = matchingStations.Single();

        sw.InspectStationID(si, s);
        sw.InspectIcaoID(si, s, true);
        sw.InspectCountry(si, s, true);
        sw.InspectDateFrom(si);
        sw.InspectDateTo(si);
        sw.InspectStationType(si);
        sw.InspectStationOwner(si);
        sw.InspectSource(s);
    }

    sw.PrintLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
}