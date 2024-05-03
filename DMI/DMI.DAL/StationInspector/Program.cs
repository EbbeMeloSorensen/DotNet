using Npgsql;
using StationInspector;

const bool includeStationsFromDenmark = true;
const bool includeStationsFromGreenland = false;
const bool includeStationsFromFaroeIslands = false;
const bool includeStationsWithoutCountry = false;

const bool omitValidRecords = true;

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

var sms_host = "172.25.7.23:5432";
var sms_user = "ebs";
var sms_password = "Vm6PAkPh";
var sms_database = "sms_prod";
var sms_connectionString = $"Host={sms_host};Username={sms_user};Password={sms_password};Database={sms_database}";

using var sms_conn = new NpgsqlConnection(sms_connectionString);
sms_conn.Open();

var stationInformations = new List<StationInformation>();

try
{
    var sms_query = 
        "SELECT " +
        "objectid, " +
        "stationname, " +
        "stationid_dmi, " +
        "stationtype, " +
        "accessaddress, " +
        "country, " +
        "status, " +
        "datefrom, " +
        "dateto, " +
        "stationowner, " +
        "stationid_icao, " +
        "hha, " +
        "wgs_lat, " +
        "wgs_long " +
        "FROM sde.stationinformation " +
        "WHERE gdb_to_date = '9999-12-31 23:59:59.000000'" +
        //" AND stationtype IN (0, 5)" +
        " AND stationtype IN (0)" +
        " AND status = 1" +
        " ORDER BY stationid_dmi";// +
        //" LIMIT 10";

    var rowCount = 0;

    using (var sms_cmd = new NpgsqlCommand(sms_query, sms_conn))
    using (var sms_reader = sms_cmd.ExecuteReader())
    {
        while (sms_reader.Read())
        {
            rowCount++;

            var objectId = sms_reader.GetInt32(0);
            string? stationname = sms_reader.IsDBNull(1) ? null : sms_reader.GetString(1);
            int? stationid_dmi = sms_reader.IsDBNull(2) ? null : sms_reader.GetInt32(2);
            int? stationtype = sms_reader.IsDBNull(3) ? null : sms_reader.GetInt32(3);
            string? accessaddress = sms_reader.IsDBNull(4) ? null : sms_reader.GetString(4);
            int? country = sms_reader.IsDBNull(5) ? null : sms_reader.GetInt32(5);
            int? status = sms_reader.IsDBNull(6) ? null : sms_reader.GetInt32(6);
            DateTime? datefrom = sms_reader.IsDBNull(7) ? null : sms_reader.GetDateTime(7);
            DateTime? dateto = sms_reader.IsDBNull(8) ? null : sms_reader.GetDateTime(8);
            int? stationowner = sms_reader.IsDBNull(9) ? null : sms_reader.GetInt32(9);
            string? stationid_icao = sms_reader.IsDBNull(10) ? null : sms_reader.GetString(10);
            double? hha = sms_reader.IsDBNull(11) ? null : sms_reader.GetDouble(11);
            double? wgs_lat = sms_reader.IsDBNull(12) ? null : sms_reader.GetDouble(12);
            double? wgs_long = sms_reader.IsDBNull(13) ? null : sms_reader.GetDouble(13);

            var stationInformation = new StationInformation
            {
                objectid = objectId,
                stationname = stationname,
                stationid_dmi = stationid_dmi,
                stationtype = stationtype,
                accessaddress = accessaddress,
                country = country,
                status = status,
                datefrom = datefrom,
                dateto = dateto,
                stationowner = stationowner,
                stationid_icao = stationid_icao,
                hha = hha,
                wgs_lat = wgs_lat,
                wgs_long = wgs_long
            };

            stationInformations.Add(stationInformation);
        }
    }
}
catch (PostgresException excp)
{
    throw excp;
}

var statdb_host = "nanoq.dmi.dk";
var statdb_user = "ebs";
var statdb_password = "Vm6PAkPh";
var statdb_database = "statdb";
var statdb_connectionString = $"Host={statdb_host};Username={statdb_user};Password={statdb_password};Database={statdb_database}";

using (var streamWriter = new StreamWriter("output.txt"))
foreach (var stationInformation in stationInformations)
{
    var stationNameAsString = stationInformation.stationname != null ? $"{stationInformation.stationname}" : "<null>";
    var stationIdDMIAsString = stationInformation.stationid_dmi.HasValue ? $"{stationInformation.stationid_dmi.Value}" : "<null>";
    var stationTypeAsString = stationInformation.stationtype.HasValue ? $"{_stationTypeMap[stationInformation.stationtype.Value]}" : "<null>";
    var accessaddressAsString = stationInformation.accessaddress != null ? stationInformation.accessaddress : "<null>";
    var countryAsString = stationInformation.country.HasValue ? $"{_countryMap[stationInformation.country.Value]}" : "<null>";
    var statusAsString = stationInformation.status.HasValue ? $"{_statusMap[stationInformation.status.Value]}" : "<null>";
    var dateFromAsString = stationInformation.datefrom.HasValue ? $"{stationInformation.datefrom.Value.AsDateTimeString(true)}" : "<null>";
    var dateToAsString = stationInformation.dateto.HasValue ? $"{stationInformation.dateto.Value.AsDateTimeString(true)}" : "<null>";
    var stationOwnerAsString = stationInformation.stationowner.HasValue ? $"{_stationOwnerMap[stationInformation.stationowner.Value]}" : "<null>";
    var stationIdIcaoAsString = stationInformation.stationid_icao != null ? stationInformation.stationid_icao : "<null>";
    var hhaAsString = stationInformation.hha != null ? $"{stationInformation.hha}" : "<null>";
    var wgsLatAsString = stationInformation.wgs_lat != null ? $"{stationInformation.wgs_lat}" : "<null>";
    var wgsLongAsString = stationInformation.wgs_long != null ? $"{stationInformation.wgs_long}" : "<null>";

    if ((countryAsString == "Danmark" && !includeStationsFromDenmark) ||
        (countryAsString == "Grønland" && !includeStationsFromGreenland)||
        (countryAsString == "Færøerne" && !includeStationsFromFaroeIslands)||
        (countryAsString == "<null>" && !includeStationsWithoutCountry))
    {
        continue;
    }

    PrintLine(streamWriter, $"{stationIdDMIAsString} ({stationNameAsString} - {stationTypeAsString})");

    try
    {
        using var statdb_conn = new NpgsqlConnection(statdb_connectionString);
        statdb_conn.Open();

        var statdb_query = $"SELECT COUNT(statid) FROM station WHERE statid::TEXT LIKE('{stationInformation.stationid_dmi}__')";

        var matchingStationCount = 0;

        using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
        {
            matchingStationCount = Convert.ToInt32(statdb_cmd.ExecuteScalar());
        }

        if (matchingStationCount == 0)
        {
            PrintLine(streamWriter,  $"NO MATCHING STATION IN STATDB");
        }
        else if (matchingStationCount == 1)
        {
            // STATION TABLE

            statdb_query = 
                "SELECT " + 
                "statid, " + 
                "icao_id, " + 
                "country, " + 
                "source " + 
                "FROM station " + 
                $"WHERE statid::TEXT LIKE('{stationInformation.stationid_dmi}__');";

            int? statid = null;
            string? icao_id = null;
            string? country = null;
            string? source = null;

            using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
            using (var statdb_reader = statdb_cmd.ExecuteReader())
            {
                if (statdb_reader.Read())
                {
                    statid = statdb_reader.IsDBNull(0) ? null : statdb_reader.GetInt32(0);
                    icao_id = statdb_reader.IsDBNull(1) ? null : statdb_reader.GetString(1);
                    country = statdb_reader.IsDBNull(2) ? null : statdb_reader.GetString(2);
                    source = statdb_reader.IsDBNull(3) ? null : statdb_reader.GetString(3);
                }
            }

            var icaoIdMatches = ICAOIDMatches(stationInformation.stationid_icao, icao_id) ? "ok" : "INVALID";
            var countryMatches = CountryMatches(stationInformation.country, country) ? "ok" : "INVALID";
            var sourceOK = source == "ing" ? "ok" : "INVALID";

            var statidAsString = statid.HasValue ? $"{statid.Value}" : "<null>";
            var icao_idAsString = !string.IsNullOrEmpty(icao_id) ? icao_id : "<null>";
            var statdb_countryAsString = !string.IsNullOrEmpty(country) ? $"{country}" : "<null>";
            var sourceAsString = !string.IsNullOrEmpty(source) ? $"{source}" : "<null>";

            PrintLine(streamWriter,  "");
            PrintLine(streamWriter,  $"    station id:                   {stationIdDMIAsString, 25} {statidAsString, 25}");
            PrintLine(streamWriter,  $"    icao id:                      {stationIdIcaoAsString, 25} {icao_idAsString, 25}   ({icaoIdMatches})");
            PrintLine(streamWriter,  $"    country:                      {countryAsString, 25} {statdb_countryAsString, 25}   ({countryMatches})");
            PrintLine(streamWriter,  $"    source:                       {"", 25} {sourceAsString, 25}   ({sourceOK})");
            PrintLine(streamWriter,  $"    datefrom (sms):               {$"{dateFromAsString}", 25} {"", 25}");
            PrintLine(streamWriter,  $"    dateto   (sms):               {$"{dateToAsString}", 25} {"", 25}");
            PrintLine(streamWriter,  $"    station owner:                {$"{stationOwnerAsString}", 25} {"", 25}");

            // NAME TABLE

            string? name = null;

            statdb_query = 
                "SELECT name " +
                "FROM name " +
                $"WHERE name.statid::TEXT LIKE('{stationInformation.stationid_dmi}__')" +
                "ORDER BY start_time DESC " +
                "LIMIT 1";

            using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
            using (var statdb_reader = statdb_cmd.ExecuteReader())
            {
                if (statdb_reader.Read())
                {
                    name = statdb_reader.IsDBNull(0) ? null : statdb_reader.GetString(0);
                }
            }

            var nameMatches = NameMatches(stationInformation.stationname, name) ? "ok" : "INVALID";

            var nameAsString = !string.IsNullOrEmpty(name) ? $"{name}" : "<null>";
            PrintLine(streamWriter,  $"    name:                         {stationNameAsString, 25} {nameAsString, 25}   ({nameMatches})");

            // ACTION TABLE

            bool? active = null;

            statdb_query = 
                "SELECT active " +
                "FROM active " +
                $"WHERE active.statid::TEXT LIKE('{stationInformation.stationid_dmi}__')" +
                "ORDER BY start_time DESC " +
                "LIMIT 1";

            using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
            using (var statdb_reader = statdb_cmd.ExecuteReader())
            {
                if (statdb_reader.Read())
                {
                    active = statdb_reader.IsDBNull(0) ? null : statdb_reader.GetBoolean(0);
                }
            }

            if (!active.HasValue)
            {
                throw new InvalidDataException("Should not be possible");
            }

            var activeMatches = ActiveMatches(stationInformation.status, active.Value) ? "ok" : "INVALID";;

            var activeAsString = active.Value == true ? "active" : "inactive";
            PrintLine(streamWriter,  $"    status:                       {statusAsString, 25} {activeAsString, 25}   ({activeMatches})");

            // POSITION TABLE
            string? entity = null;
            DateTime? start_time = null;
            double? latitude = null;
            double? longitude = null;
            double? height = null;

            statdb_query = 
                "SELECT entity, start_time, lat, long, height " +
                "FROM position " +
                $"WHERE position.statid::TEXT LIKE('{stationInformation.stationid_dmi}__') " +
                "AND end_time = 'infinity' " +
                "ORDER BY start_time DESC " +
                "LIMIT 1";

            using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
            using (var statdb_reader = statdb_cmd.ExecuteReader())
            {
                if (statdb_reader.Read())
                {
                    entity = statdb_reader.IsDBNull(0) ? null : statdb_reader.GetString(0);
                    start_time = statdb_reader.IsDBNull(1) ? null : statdb_reader.GetDateTime(1);
                    latitude = statdb_reader.IsDBNull(2) ? null : statdb_reader.GetDouble(2);
                    longitude = statdb_reader.IsDBNull(3) ? null : statdb_reader.GetDouble(3);
                    height = statdb_reader.IsDBNull(4) ? null : statdb_reader.GetDouble(4);

                    if (!latitude.HasValue ||
                        !longitude.HasValue)
                    {
                        throw new InvalidDataException("Should not be possible");
                    }

                    var entityAsString = !string.IsNullOrEmpty(entity) ? $"{entity}" : "<null>";
                    var startTimeAsString = start_time.HasValue ? start_time.Value.AsDateTimeString(false) : "<null>";
                    var latitudeAsString = $"{latitude.Value}";
                    var longitudeAsString = $"{longitude.Value}";
                    var heightAsString = height.HasValue ? $"{height}" : "<null>";

                    var entityOK = entity == "station" ? "ok" : "INVALID";
                    var latitudeMatches = ValueMatches(stationInformation.wgs_lat, latitude, out var latDiff, 0.00001) ? "ok" : "INVALID";
                    var longitudeMatches = ValueMatches(stationInformation.wgs_long, longitude, out var longDiff, 0.00001) ? "ok" : "INVALID";
                    var heightMatches = ValueMatches(stationInformation.hha, height, out var heightDiff, 0.00001) ? "ok" : "INVALID";

                    if (latitudeMatches == "INVALID" && latDiff.HasValue)
                    {
                        latitudeMatches += $" (difference: {latDiff.Value})";
                    }

                    if (longitudeMatches == "INVALID" && longDiff.HasValue)
                    {
                        longitudeMatches += $" (difference: {longDiff.Value})";
                    }

                    if (heightMatches == "INVALID" && heightDiff.HasValue)
                    {
                        heightMatches += $" (difference: {heightDiff.Value})";
                    }

                    PrintLine(streamWriter,  $"    entity:                       {"", 25} {entityAsString, 25}   ({entityOK})");
                    PrintLine(streamWriter,  $"    start_time (latest position): {"", 25} {startTimeAsString, 25}");
                    PrintLine(streamWriter,  $"    latitude:                     {wgsLatAsString, 25} {latitudeAsString, 25}   ({latitudeMatches})");
                    PrintLine(streamWriter,  $"    longitude:                    {wgsLongAsString, 25} {longitudeAsString, 25}   ({longitudeMatches})");
                    PrintLine(streamWriter,  $"    height:                       {hhaAsString, 25} {heightAsString, 25}   ({heightMatches})");
                }
                else
                {
                    PrintLine(streamWriter,  $"    latitude:                     {wgsLatAsString, 25} {"", 25}   ({"INVALID (NO POSITION IN STATDB"})");
                    PrintLine(streamWriter,  $"    longitude:                    {wgsLongAsString, 25} {"", 25}   ({"INVALID (NO POSITION IN STATDB"})");
                    PrintLine(streamWriter,  $"    height:                       {hhaAsString, 25} {"", 25}   ({"INVALID (NO POSITION IN STATDB"})");
                }
            }
        }
        else
        {
            PrintLine(streamWriter,  $"MULTIPLE MATCHING STATIONS IN STATDB");
        }

        PrintLine(streamWriter, "---------------------------------------------------------------");
    }
    catch (PostgresException excp)
    {
        throw excp;
    }
}

static void PrintLine(
    StreamWriter streamWriter, 
    string line)
{
    Console.WriteLine(line);
    streamWriter.WriteLine(line);
}

static bool ICAOIDMatches(
    string? icaoIdInSMS,
    string? icaoIdInStatDB)
{
    if (icaoIdInSMS == null)
    {
        return string.IsNullOrEmpty(icaoIdInStatDB);
    }

    if (string.IsNullOrEmpty(icaoIdInStatDB))
    {
        return false;
    }

    return icaoIdInSMS == icaoIdInStatDB;
}

static bool CountryMatches(
    int? countryCodeInSMS,
    string? countryInStatDB)
{
    if (!countryCodeInSMS.HasValue)
    {
        return countryInStatDB == null;
    }

    if (countryInStatDB == null)
    {
        return false;
    }

    var countryMap = new Dictionary<int, string>
    {
        { 0, "Danmark" },
        { 1, "Grønland" },
        { 2, "Færøerne" }
    };

    return countryInStatDB == countryMap[countryCodeInSMS.Value];
}

static bool NameMatches(
    string? nameInSMS,
    string? nameInStatDB)
{
    if (nameInSMS == null)
    {
        return nameInStatDB == null;
    }

    if (nameInStatDB == null)
    {
        return false;
    }

    return nameInStatDB == nameInSMS.ToUpper();
}

static bool ActiveMatches(
    int? statusInSMS,
    bool activeInStatDB)
{
    if (!statusInSMS.HasValue)
    {
        return false;
    }

    if (statusInSMS.Value == 0)
    {
        return activeInStatDB == false;
    }
    else if (statusInSMS.Value == 1)
    {
        return activeInStatDB == true;
    }
    else
    {
        throw new InvalidDataException("Should not occur");
    }
}

static bool ValueMatches(
    double? smsCoordinate,
    double? statDBCoordinate,
    out double? difference,
    double tolerance)
{
    difference = null;

    if (!smsCoordinate.HasValue)
    {
        return !statDBCoordinate.HasValue;
    }

    if (!statDBCoordinate.HasValue)
    {
        return false;
    }

    difference = Math.Abs(smsCoordinate.Value - statDBCoordinate.Value);

    return difference < tolerance;
}

internal static class Helpers
{
    public static string AsDateString(
        this DateTime dateTime)
    {
        var year = dateTime.Year;
        var month = dateTime.Month.ToString().PadLeft(2, '0');
        var day = dateTime.Day.ToString().PadLeft(2, '0');

        var result = $"{year}-{month}-{day}";

        return result;
    }

    public static string AsDateTimeString(
        this DateTime dateTime,
        bool includeMilliseconds)
    {
        var year = dateTime.Year;
        var month = dateTime.Month.ToString().PadLeft(2, '0');
        var day = dateTime.Day.ToString().PadLeft(2, '0');

        var hour = dateTime.Hour.ToString().PadLeft(2, '0');
        var minute = dateTime.Minute.ToString().PadLeft(2, '0');
        var second = dateTime.Second.ToString().PadLeft(2, '0');

        var result = $"{year}-{month}-{day} {hour}:{minute}:{second}";

        if (includeMilliseconds)
        {
            var millisecond = dateTime.Millisecond.ToString().PadLeft(3, '0');

            result += $".{millisecond}";
        }

        return result;
    }
}