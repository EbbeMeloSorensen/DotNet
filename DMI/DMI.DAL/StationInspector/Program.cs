using Npgsql;
using StationInspector;

const bool includeStationsFromDenmark = true;
const bool includeStationsFromGreenland = false;
const bool includeStationsFromFaroeIslands = false;
const bool includeStationsWithoutCountry = false;

const bool includeSynopStations = false;
const bool includePluvioStations = true;
const bool includeSVKStations = false;

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

var checkStartTimeReportType = false; // (should match sms::stationinformation::datefrom)
var checkStartTimeLatestPosition = false; // (should not be older than sms::stationinformation::datefrom)
var _bizRuleViolations = new Dictionary<string, int>(); 

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
    var stationTypeFilter = new List<int>();

    if (includeSynopStations)
    {
        stationTypeFilter.Add(0);
    }

    if (includeSVKStations)
    {
        stationTypeFilter.Add(2);
    }

    if (includePluvioStations)
    {
        stationTypeFilter.Add(5);
    }

    var stationTypeFilterAsCSV = stationTypeFilter
        .Select(_ => _.ToString())
        .Aggregate((c, n) => $"{c}, {n}");

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
        $" AND stationtype IN ({stationTypeFilterAsCSV})" +
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

var statdb_parameter_host = "nanoq.dmi.dk";
//var statdb_parameter_host = "nanoqt.dmi.dk";
var statdb_parameter_user = "ebs";
var statdb_parameter_password = "Vm6PAkPh";
var statdb_parameter_database = "statdb_parameter";
var statdb_parameter_connectionString = $"Host={statdb_parameter_host};Username={statdb_parameter_user};Password={statdb_parameter_password};Database={statdb_parameter_database}";

using (var streamWriter = new StreamWriter("output.txt"))
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
                // STATION TABLE (IN STATDB)

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

                var icaoIdMatches = ICAOIDMatches(stationInformation.stationid_icao, icao_id) ? "ok" : "INVALID (DIFFERS FROM SMS)";
                var countryMatches = CountryMatches(stationInformation.country, country) ? "ok" : "INVALID";
                var sourceOK = source == "ing" ? "ok" : "INVALID";
                var dateFromOK = dateFromAsString != "<null>" ? "ok" : "INVALID";
                var dateToOK = dateToAsString == "<null>" ? "ok" : "INVALID";
                var statidAsString = statid.HasValue ? $"{statid.Value}" : "<null>";
                var icao_idAsString = !string.IsNullOrEmpty(icao_id) ? icao_id : "<null>";
                var statdb_countryAsString = !string.IsNullOrEmpty(country) ? $"{country}" : "<null>";
                var sourceAsString = !string.IsNullOrEmpty(source) ? $"{source}" : "<null>";

                PrintLine(streamWriter,  "");
                PrintLine(streamWriter,  $"    station id:                   {stationIdDMIAsString, 40} {statidAsString, 40}");
                PrintLine(streamWriter,  $"    icao id:                      {stationIdIcaoAsString, 40} {icao_idAsString, 40}   ({icaoIdMatches})");
                PrintLine(streamWriter,  $"    country:                      {countryAsString, 40} {statdb_countryAsString, 40}   ({countryMatches})");
                PrintLine(streamWriter,  $"    source:                       {"", 40} {sourceAsString, 40}   ({sourceOK})");
                PrintLine(streamWriter,  $"    datefrom (sms):               {$"{dateFromAsString}", 40} {"", 40}   ({dateFromOK})");
                PrintLine(streamWriter,  $"    dateto   (sms):               {$"{dateToAsString}", 40} {"", 40}   ({dateToOK})");
                PrintLine(streamWriter,  $"    station type:                 {$"{stationTypeAsString}", 40} {"", 40}");
                PrintLine(streamWriter,  $"    station owner:                {$"{stationOwnerAsString}", 40} {"", 40}");

                // NAME TABLE (IN STATDB)

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

                var nameMatches = NameMatches(stationInformation.stationname, name) ? "ok" : "INVALID (DIFFERS FROM NAME IN SMS)";

                var nameAsString = !string.IsNullOrEmpty(name) ? $"{name}" : "<null>";
                PrintLine(streamWriter,  $"    name:                         {stationNameAsString, 40} {nameAsString, 40}   ({nameMatches})");

                // ACTION TABLE (IN STATDB)

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

                var activeMatches = ActiveMatches(stationInformation.status, active.Value) ? "ok" : "INVALID (DIFFERS FROM STATUS IN SMS)";

                var activeAsString = active.Value == true ? "active" : "inactive";
                PrintLine(streamWriter,  $"    status:                       {statusAsString, 40} {activeAsString, 40}   ({activeMatches})");

                // POSITION TABLE (IN STATDB)
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

                        var startTimeLatestPositionOK = 
                            start_time.HasValue &&
                            (!stationInformation.datefrom.HasValue ||
                            stationInformation.datefrom.Value <= start_time.Value) ? "ok" : "INVALID (SHOULD NOT BE OLDER THAN DATEFROM IN SMS)";

                        if (startTimeLatestPositionOK != "ok" && checkStartTimeLatestPosition)
                        {
                            AppendBizRuleViolation("start_time of latest position in statdb should not be earlier than sms::stationinformation::datefrom");
                        }

                        if (latitudeMatches == "INVALID")
                        {
                            if (latDiff.HasValue)
                            {
                                latitudeMatches += $" (DIFFERS FROM SMS BY {latDiff.Value})";
                            }
                            else
                            {
                                latitudeMatches += " (DIFFERS FROM SMS)";
                            }
                        }

                        if (longitudeMatches == "INVALID")
                        {
                            if (longDiff.HasValue)
                            {
                                longitudeMatches += $" (DIFFERS FROM SMS BY {longDiff.Value})";
                            }
                            else
                            {
                                longitudeMatches += " (DIFFERS FROM SMS)";
                            }
                        }

                        if (heightMatches == "INVALID")
                        {
                            if (heightDiff.HasValue)
                            {
                                heightMatches += $" (DIFFERS FROM SMS BY {heightDiff.Value})";
                            }
                            else
                            {
                                heightMatches += " (DIFFERS FROM SMS)";
                            }
                        }

                        PrintLine(streamWriter,  $"    entity:                       {"", 40} {entityAsString, 40}   ({entityOK})");

                        var line1 = $"    start_time (latest position): {"", 40} {startTimeAsString, 40}";

                        if (checkStartTimeLatestPosition)
                        {
                            line1 = $"{line1}   ({startTimeLatestPositionOK})";
                        }

                        PrintLine(streamWriter,  line1);
                        PrintLine(streamWriter,  $"    latitude:                     {wgsLatAsString, 40} {latitudeAsString, 40}   ({latitudeMatches})");
                        PrintLine(streamWriter,  $"    longitude:                    {wgsLongAsString, 40} {longitudeAsString, 40}   ({longitudeMatches})");
                        PrintLine(streamWriter,  $"    height:                       {hhaAsString, 40} {heightAsString, 40}   ({heightMatches})");
                    }
                    else
                    {
                        PrintLine(streamWriter,  $"    latitude:                     {wgsLatAsString, 40} {"", 40}   ({"INVALID (NO POSITION IN STATDB"})");
                        PrintLine(streamWriter,  $"    longitude:                    {wgsLongAsString, 40} {"", 40}   ({"INVALID (NO POSITION IN STATDB"})");
                        PrintLine(streamWriter,  $"    height:                       {hhaAsString, 40} {"", 40}   ({"INVALID (NO POSITION IN STATDB"})");
                    }
                }

                // REPORT_TYPE TABLE (in STATDB)

                string? report_type = null;
                DateTime? start_time_report_type = null;
                string? frequency = null;

                statdb_query = 
                    "SELECT report_type, start_time, frequency " +
                    "FROM report_type " +
                    $"WHERE report_type.statid::TEXT LIKE('{stationInformation.stationid_dmi}__') " +
                    "AND end_time = 'infinity' " +
                    "ORDER BY start_time DESC " +
                    "LIMIT 1";

                using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
                using (var statdb_reader = statdb_cmd.ExecuteReader())
                {
                    if (statdb_reader.Read())
                    {
                        report_type = statdb_reader.IsDBNull(0) ? null : statdb_reader.GetString(0);
                        start_time_report_type = statdb_reader.IsDBNull(1) ? null : statdb_reader.GetDateTime(1);
                        frequency = statdb_reader.IsDBNull(2) ? null : statdb_reader.GetString(2);
                    }
                }

                var reportTypeOK = (!string.IsNullOrEmpty(report_type) && report_type == "synop") ? "ok" : "SHOULD BE SYNOP";

                var startTimeReportTypeOK =
                    start_time_report_type.HasValue &&
                    (!stationInformation.datefrom.HasValue ||
                        start_time_report_type.Value == stationInformation.datefrom.Value
                    ) ? "ok" : "INVALID (SHOULD MATCH DATEFROM IN SMS)";

                if (checkStartTimeReportType && startTimeReportTypeOK != "ok")
                {
                    AppendBizRuleViolation("statdb::report_type::start_time should match sms::stationinformation::datefrom");
                }

                var reportTypeAsString = !string.IsNullOrEmpty(report_type) ? report_type : "<null>";
                var startTimeReportTypeAsString = start_time_report_type.HasValue ? start_time_report_type.Value.AsDateTimeString(false) : "<null>";
                var frequencyAsString = !string.IsNullOrEmpty(frequency) ? frequency : "<null>";

                PrintLine(streamWriter,  $"    report_type:                  {"", 40} {reportTypeAsString, 40}   ({reportTypeOK})");

                var line = $"    start_time (report type):     {"", 40} {startTimeReportTypeAsString, 40}";

                if (checkStartTimeReportType)
                {
                    line += $"   ({startTimeReportTypeOK})";
                }

                PrintLine(streamWriter, line);

                // STAT_OBS_CODE TABLE (IN STATDB_PARAMETER)

                using var statdb_parameter_conn = new NpgsqlConnection(statdb_parameter_connectionString);
                statdb_parameter_conn.Open();

                DateTime? first_date = null;
                string? obs_code = null;

                var statdb_parameter_query = 
                    "SELECT first_date, obs_code " +
                    "FROM stat_obs_code " +
                    $"WHERE stat_obs_code.statid::TEXT LIKE('{stationInformation.stationid_dmi}__') " +
                    "AND last_date = 'infinity' " +
                    "ORDER BY first_date DESC " +
                    "LIMIT 1";

                using (var statdb_parameter_cmd = new NpgsqlCommand(statdb_parameter_query, statdb_parameter_conn))
                using (var statdb_parameter_reader = statdb_parameter_cmd.ExecuteReader())
                {
                    if (statdb_parameter_reader.Read())
                    {
                        first_date = statdb_parameter_reader.IsDBNull(0) ? null : statdb_parameter_reader.GetDateTime(0);
                        obs_code = statdb_parameter_reader.IsDBNull(1) ? null : statdb_parameter_reader.GetString(1);

                        var firstDateAsString = first_date.HasValue ? first_date.Value.AsDateTimeString(false) : "<null>";
                        var obsCodeAsString = !string.IsNullOrEmpty(obs_code) ? obs_code : "<null>";

                        var firstDateObsCodeOK =
                            first_date.HasValue &&
                            (!stationInformation.datefrom.HasValue ||
                                first_date.Value == stationInformation.datefrom.Value
                            ) ? "ok" : "INVALID (SHOULD MATCH DATEFROM IN SMS)";

                        var obsCodeOK = 
                            (obsCodeAsString =="pluvio" && stationInformation.stationtype == 5) ||
                            (obsCodeAsString =="PSVK" && stationInformation.stationtype == 2)
                            ? "ok" : "INVALID (SHOULD CORRESPOND TO STATION TYPE IN SMS)";

                        PrintLine(streamWriter,  $"    obs_code:                     {"", 40} {$"{obsCodeAsString}", 40}   ({obsCodeOK})");
                        PrintLine(streamWriter,  $"    first_date (obs_code):        {"", 40} {$"{firstDateAsString}", 40}   ({firstDateObsCodeOK})");
                    }
                    else
                    {
                        PrintLine(streamWriter,  " NO OBS_CODE IN STATDB_PARAMETER FOR STATION");
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

    // Log summary of violated business rules
    PrintLine(streamWriter, "");
    PrintLine(streamWriter, "SUMMARY OF DISCREPANCIES");

    foreach (var kvp in _bizRuleViolations)
    {
        PrintLine(streamWriter, $"{kvp.Key, 50}: {kvp.Value, 10}");
    }
}


void AppendBizRuleViolation(
    string businessRule)
{
    if (!_bizRuleViolations.ContainsKey(businessRule))
    {
        _bizRuleViolations[businessRule] = 0;
    }

    _bizRuleViolations[businessRule]++;
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