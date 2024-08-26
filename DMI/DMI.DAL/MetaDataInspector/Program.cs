using MetaDataInspector;
using MetaDataInspector.Inspectors;

// Country filter for SMS
const bool includeStationsFromDenmark = true;
const bool includeStationsFromGreenland = false;
const bool includeStationsFromFaroeIslands = false;
const bool includeStationsWithoutCountry = false;

// Station Type filter for SMS
const bool includeSynopStations = true;
const bool includeSVKStations = true;
const bool includePluvioStations = true;

// Evaluation filter 
const bool evaluateNames = true;
const bool evaluateStatuses = true;
const bool evaluateCountries = false;
const bool evaluateIcaoIDs = false;
const bool evaluateLatitudes = true;
const bool evaluateLongitudes = true;
const bool evaluateHeights = true;

var stations = StatDB.RetrieveStations();
var names = StatDB.RetrieveNames();
var statuses = StatDB.RetrieveStatuses();
var positions = StatDB.RetrievePositions();

var stationInformations = SMS.RetrieveStationInformations(
    includeSynopStations,
    includeSVKStations,
    includePluvioStations);

var missingStations = 0;
var nameMismatches = 0;
var statusMismatches = 0;
var countryMismatches = 0;
var icaoIDMismatches = 0;
var latitudeMismatches = 0;
var longitudeMismatches = 0;
var heightMismatches = 0;

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
        missingStations++;
    }
    else if (matchingStationCount > 1)
    {
        sw.PrintLine("MULTIPLE MATCHING STATIONS IN STATDB");
    }
    else
    {
        var s = matchingStations.Single();

        sw.InspectStationID(si, s);
        if (!sw.InspectIcaoID(si, s, evaluateIcaoIDs)) { icaoIDMismatches++; };
        if (!sw.InspectCountry(si, s, evaluateCountries)) { countryMismatches++; };
        sw.InspectSource(s);
        sw.InspectDateFrom(si);
        sw.InspectDateTo(si);
        sw.InspectStationType(si);
        sw.InspectStationOwner(si);
        if (!sw.InspectName(si, names, evaluateNames)) { nameMismatches++; };
        if (!sw.InspectStatus(si, statuses, evaluateStatuses)) { statusMismatches++; };
        if (!sw.InspectLatitude(si, positions, evaluateLatitudes, tolerance: 0.00001)) { latitudeMismatches++; };
        if (!sw.InspectLongitude(si, positions, evaluateLongitudes, tolerance: 0.00001)) { longitudeMismatches++; };
        if (!sw.InspectHeight(si, positions, evaluateHeights, tolerance: 0.00001)) { heightMismatches++; };
    }

    sw.PrintLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
}

sw.PrintLine("Summary:");
sw.PrintLine("");
sw.PrintLine($"  Stations inspected in SMS: {stationInformations.Count(),4}");
sw.PrintLine($"          Missing in statdb: {missingStations,4}");

if (evaluateNames)
{
    sw.PrintLine($"            Name mismatches: {nameMismatches,4}");
}

if (evaluateStatuses)
{
    sw.PrintLine($"          Status mismatches: {statusMismatches,4}");
}

if (evaluateCountries)
{
    sw.PrintLine($"         Country mismatches: {countryMismatches,4}");
}

if (evaluateIcaoIDs)
{
    sw.PrintLine($"         ICAO ID mismatches: {icaoIDMismatches,4}");
}

if (evaluateLatitudes) 
{
    sw.PrintLine($"        Latitude mismatches: {latitudeMismatches,4}"); 
}

if (evaluateLongitudes) 
{
    sw.PrintLine($"       Longitude mismatches: {longitudeMismatches,4}");
}

if (evaluateHeights)
{
    sw.PrintLine($"          Height mismatches: {heightMismatches,4}");
}

sw.PrintLine("-----------------------------------------------------------------------------------------------------------------------------------------------");
