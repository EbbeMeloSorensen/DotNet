using MetaDataInspector;

const bool includeStationsFromDenmark = true;
const bool includeStationsFromGreenland = false;
const bool includeStationsFromFaroeIslands = false;
const bool includeStationsWithoutCountry = false;

var _countryMap = new Dictionary<int, string>{
    { 0, "Danmark" },
    { 1, "Grønland" },
    { 2, "Færøerne" }
};

var _statusMap = new Dictionary<int, string>{
    { 0, "inactive" },
    { 1, "active" }
};

var _stationTypeMap = new Dictionary<int, string>{
    { 0, "Synop" },
    { 1, "Strømstation" },
    { 2, "SVK gprs" },
    { 3, "Vandstandsstation" },
    { 4, "GIWS" },
    { 5, "Pluvio" },
    { 6, "SHIP AWS" },
    { 7, "Temp ship" },
    { 8, "Lynpejlestation" },
    { 9, "Radar" },
    { 10, "Radiosonde" },
    { 11, "Historisk stationstype" },
    { 12, "Manuel Nedbør" },
    { 13, "Bølgestation" },
    { 14, "Snestation" }
};

var _stationOwnerMap = new Dictionary<int, string>{
    { 0, "DMI" },
    { 1, "SVK" },
    { 2, "Havne Kommuner mm" },
    { 3, "GC-net" },
    { 4, "Danske lufthavne" },
    { 5, "MITT/FRL lufthavne" },
    { 6, "Vejdirektoratet" },
    { 7, "Synop - Aarhus Uni" },
    { 8, "Asiaq" },
    { 9, "Kystdirektoratet" },
    { 10, "PROMICE" },
    { 11, "Forsvaret" }
};

var stationInformations = DataHelpers.RetrieveStationInformations(
    includeSynopStations: false,
    includeSVKStations: false,
    includePluvioStations: true);

using (var streamWriter = new StreamWriter("output2.txt"))
{
    foreach (var stationInformation in stationInformations)
    {
        var stationNameAsString = stationInformation.stationname != null ? $"{stationInformation.stationname}" : "<null>";
        var stationIdDMIAsString = stationInformation.stationid_dmi.HasValue ? $"{stationInformation.stationid_dmi.Value}" : "<null>";
        var stationTypeAsString = stationInformation.stationtype.HasValue ? $"{_stationTypeMap[stationInformation.stationtype.Value]}" : "<null>";
        var accessaddressAsString = stationInformation.accessaddress != null ? stationInformation.accessaddress : "<null>";
        var countryAsString = stationInformation.country.HasValue ? $"{_countryMap[stationInformation.country.Value]}" : "<null>";
        var statusAsString = stationInformation.status.HasValue ? $"{_statusMap[stationInformation.status.Value]}" : "<null>";
        var dateFromAsString = stationInformation.datefrom.HasValue ? $"{stationInformation.datefrom.Value.AsDateTimeString(false)}" : "<null>";
        var dateToAsString = stationInformation.dateto.HasValue ? $"{stationInformation.dateto.Value.AsDateTimeString(false)}" : "<null>";
        var stationOwnerAsString = stationInformation.stationowner.HasValue ? $"{_stationOwnerMap[stationInformation.stationowner.Value]}" : "<null>";
        var stationIdIcaoAsString = stationInformation.stationid_icao != null ? stationInformation.stationid_icao : "<null>";
        var hhaAsString = stationInformation.hha != null ? $"{stationInformation.hha}" : "<null>";
        var wgsLatAsString = stationInformation.wgs_lat != null ? $"{stationInformation.wgs_lat}" : "<null>";
        var wgsLongAsString = stationInformation.wgs_long != null ? $"{stationInformation.wgs_long}" : "<null>";

        if ((countryAsString == "Danmark" && !includeStationsFromDenmark) ||
            (countryAsString == "Grønland" && !includeStationsFromGreenland) ||
            (countryAsString == "Færøerne" && !includeStationsFromFaroeIslands) ||
            (countryAsString == "<null>" && !includeStationsWithoutCountry))
        {
            continue;
        }

        streamWriter.PrintLine($"{stationIdDMIAsString} ({stationNameAsString} - {stationTypeAsString})");
    }
}