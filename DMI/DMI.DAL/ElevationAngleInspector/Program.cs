using ElevationAngleInspector;
using Npgsql;

var sms_host = "172.25.7.23:5432";
var sms_user = "ebs";
var sms_password = "Vm6PAkPh";
var sms_database = "sms_prod";
var sms_connectionString = $"Host={sms_host};Username={sms_user};Password={sms_password};Database={sms_database}";

using var sms_conn = new NpgsqlConnection(sms_connectionString);
sms_conn.Open();

var sms_stations = new List<SMS_Station>();

try
{
    var sms_query = 
        "SELECT " + 
        "sde.sensorlocation.stationid_dmi, " + 
        "sde.elevationangles.datefrom, " + 
        "sde.elevationangles.angle_n, " + 
        "sde.elevationangles.angle_ne, " + 
        "sde.elevationangles.angle_e, " + 
        "sde.elevationangles.angle_se, " + 
        "sde.elevationangles.angle_s, " + 
        "sde.elevationangles.angle_sw, " + 
        "sde.elevationangles.angle_w, " + 
        "sde.elevationangles.angle_nw, " + 
        "sde.elevationangles.angleindex " + 
        "FROM sde.sensorlocation " + 
        "INNER JOIN sde.elevationangles ON sde.sensorlocation.globalid=sde.elevationangles.parentguid " + 
        "WHERE sde.sensorlocation.gdb_to_date = '9999-12-31 23:59:59' " + 
        "AND sde.elevationangles.gdb_to_date = '9999-12-31 23:59:59' " + 
        "ORDER BY sde.sensorlocation.stationid_dmi, sde.elevationangles.datefrom ";

    using (var sms_cmd = new NpgsqlCommand(sms_query, sms_conn))
    using (var sms_reader = sms_cmd.ExecuteReader())
    {
        while (sms_reader.Read())
        {
            sms_stations.Add(new SMS_Station
            {

                stationid_dmi = sms_reader.IsDBNull(0) ? null : sms_reader.GetInt32(0),
                datefrom = sms_reader.GetDateTime(1),
                angle_n = sms_reader.IsDBNull(2) ? null : sms_reader.GetInt32(2),
                angle_ne = sms_reader.IsDBNull(3) ? null : sms_reader.GetInt32(3),
                angle_e = sms_reader.IsDBNull(4) ? null : sms_reader.GetInt32(4),
                angle_se = sms_reader.IsDBNull(5) ? null : sms_reader.GetInt32(5),
                angle_s = sms_reader.IsDBNull(6) ? null : sms_reader.GetInt32(6),
                angle_sw = sms_reader.IsDBNull(7) ? null : sms_reader.GetInt32(7),
                angle_w = sms_reader.IsDBNull(8) ? null : sms_reader.GetInt32(8),
                angle_nw = sms_reader.IsDBNull(9) ? null : sms_reader.GetInt32(9),
                angleindex = sms_reader.IsDBNull(10) ? null : sms_reader.GetInt32(10)
            });
        }
    }

    var count = sms_stations.Count; // 2988 (16-10-2023)

    sms_stations = sms_stations
        .OrderBy(_ => _.stationid_dmi)
        .ThenBy(_ => _.datefrom)
        .ToList();
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

var statdb_stations = new List<STATDB_Station>();

try
{
    using var statdb_conn = new NpgsqlConnection(statdb_connectionString);
    statdb_conn.Open();

    var statdb_query = 
        "SELECT " + 
        "station.statid, " +
        "leeindex.start_time, " +
        "leeindex.n, " +
        "leeindex.ne, " +
        "leeindex.e, " +
        "leeindex.se, " +
        "leeindex.s, " +
        "leeindex.sw, " +
        "leeindex.w, " +
        "leeindex.nw, " +
        "leeindex.index " +
        "FROM station " +
        "INNER JOIN leeindex ON station.statid=leeindex.statid " +
        "ORDER BY station.statid, start_time";

    using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
    using (var statdb_reader = statdb_cmd.ExecuteReader())
    {
        while (statdb_reader.Read())
        {
            statdb_stations.Add(new STATDB_Station{
                statid = statdb_reader.GetInt32(0),
                start_time = statdb_reader.GetDateTime(1),
                leeindex_n = statdb_reader.GetInt32(2),
                leeindex_ne = statdb_reader.GetInt32(3),
                leeindex_e = statdb_reader.GetInt32(4),
                leeindex_se = statdb_reader.GetInt32(5),
                leeindex_s = statdb_reader.GetInt32(6),
                leeindex_sw = statdb_reader.GetInt32(7),
                leeindex_w = statdb_reader.GetInt32(8),
                leeindex_nw = statdb_reader.GetInt32(9),
                leeindexindex = statdb_reader.GetInt32(10)
            });
        }
    }
}
catch (PostgresException excp)
{
    throw excp;
}

var smsReportLines = new List<SMS_Report_Line>();

// Traverse the collection of elevation angle sets from sms and compare with statdb
foreach(var sms_station in sms_stations)
{
    var smsReportLine = new SMS_Report_Line();
    smsReportLine.stationid_dmi = sms_station.stationid_dmi;
    smsReportLine.datefrom = sms_station.datefrom;
    smsReportLine.angle_n = sms_station.angle_n;
    smsReportLine.angle_ne = sms_station.angle_ne;
    smsReportLine.angle_e = sms_station.angle_e;
    smsReportLine.angle_se = sms_station.angle_se;
    smsReportLine.angle_s = sms_station.angle_s;
    smsReportLine.angle_sw = sms_station.angle_sw;
    smsReportLine.angle_w = sms_station.angle_w;
    smsReportLine.angle_nw = sms_station.angle_nw;
    smsReportLine.angleindex = sms_station.angleindex;

    if (!sms_station.stationid_dmi.HasValue)
    {
        smsReportLine.comment = "station id missing in sms database";
    }
    else
    {
        // Try to find a matching elevation angle set in the elevation angle list from statdb
        var elevationAngleSetsFromStatDB = statdb_stations
            .Where(_ => _.statid / 100 == sms_station.stationid_dmi && _.start_time == sms_station.datefrom);

        if (!elevationAngleSetsFromStatDB.Any())
        {
            smsReportLine.comment = "no elevation angle set for given station id and date in statdb";
        }
        else if (elevationAngleSetsFromStatDB.Count() > 1)
        {
            var distinctNValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_n).Distinct();
            var distinctNEValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_ne).Distinct();
            var distinctEValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_e).Distinct();
            var distinctSEValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_se).Distinct();
            var distinctSValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_s).Distinct();
            var distinctSWValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_sw).Distinct();
            var distinctWValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_w).Distinct();
            var distinctNWValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindex_nw).Distinct();
            var distinctIndexValues = elevationAngleSetsFromStatDB.Select(_ => _.leeindexindex).Distinct();

            if (distinctNValues.Count() == 1 &&
                distinctNEValues.Count() == 1 &&
                distinctEValues.Count() == 1 &&
                distinctSEValues.Count() == 1 &&
                distinctSValues.Count() == 1 &&
                distinctSWValues.Count() == 1 &&
                distinctWValues.Count() == 1 &&
                distinctNWValues.Count() == 1 &&
                distinctIndexValues.Count() == 1)
            {
                var nValue = distinctNValues.Single();
                var neValue = distinctNEValues.Single();
                var eValue = distinctEValues.Single();
                var seValue = distinctSEValues.Single();
                var sValue = distinctSValues.Single();
                var swValue = distinctSWValues.Single();
                var wValue = distinctWValues.Single();
                var nwValue = distinctNWValues.Single();
                var indexValue = distinctIndexValues.Single();

                if (sms_station.angle_n == nValue &&
                    sms_station.angle_ne == neValue &&
                    sms_station.angle_e == eValue &&
                    sms_station.angle_se == seValue &&
                    sms_station.angle_s == sValue &&
                    sms_station.angle_sw == swValue &&
                    sms_station.angle_w == wValue &&
                    sms_station.angle_nw == nwValue &&
                    sms_station.angleindex == indexValue)
                {
                    smsReportLine.comment = "match (multiple representations in statdb)";
                }
            }
            else
            {
                // This doesn't occur
                throw new NotImplementedException();
            }
        }
        else
        {
            var elevationAngleSetFromStatDB = elevationAngleSetsFromStatDB.Single();

            if (elevationAngleSetFromStatDB.leeindex_n == sms_station.angle_n &&
                elevationAngleSetFromStatDB.leeindex_ne == sms_station.angle_ne &&
                elevationAngleSetFromStatDB.leeindex_e == sms_station.angle_e &&
                elevationAngleSetFromStatDB.leeindex_se == sms_station.angle_se &&
                elevationAngleSetFromStatDB.leeindex_s == sms_station.angle_s &&
                elevationAngleSetFromStatDB.leeindex_sw == sms_station.angle_sw &&
                elevationAngleSetFromStatDB.leeindex_w == sms_station.angle_w &&
                elevationAngleSetFromStatDB.leeindex_nw == sms_station.angle_nw &&
                elevationAngleSetFromStatDB.leeindexindex == sms_station.angleindex)
            {
                //smsReportLine.comment = $"match ({elevationAngleSetFromStatDB})";
                smsReportLine.comment = "match";
            }
            else
            {
                smsReportLine.comment = $"mismatch for angle set for given station id and date (row in statdb: {elevationAngleSetFromStatDB})";
            }
        }
    }

    smsReportLines.Add(smsReportLine);
}

smsReportLines = smsReportLines
    .OrderBy(_ => _.datefrom)
    .ToList();

var firstYear = smsReportLines.Min(_ => _.datefrom.Year);
var lastYear = smsReportLines.Max(_ => _.datefrom.Year);

using (var streamWriter = new StreamWriter("elevation_angles_sms_vs_statdb.txt"))
{
    foreach (var smsReportLine in smsReportLines) 
    {
        PrintLine(streamWriter, smsReportLine.ToString());
    }

    var omitElevationAngleSetsWithoutAStationId = true;
    var year = firstYear;

    while (year <= lastYear)
    {
        var smsReportLinesForCurrentYear = smsReportLines.Where(_ => _.datefrom.Year == year) ;

        if (smsReportLinesForCurrentYear.Any())
        {
            PrintLine(streamWriter, "");
            PrintLine(streamWriter, $"Summary of elevation angle sets for {year}:");
            PrintSummary(streamWriter, smsReportLinesForCurrentYear, omitElevationAngleSetsWithoutAStationId);
        }

        year++;
    }

    PrintLine(streamWriter, "");
    PrintLine(streamWriter, "Summary of entire collection of elevation angle sets:");
    PrintSummary(streamWriter, smsReportLines, omitElevationAngleSetsWithoutAStationId);
}

static void PrintLine(
    StreamWriter streamWriter, 
    string line)
{
    Console.WriteLine(line);
    streamWriter.WriteLine(line);
}

static void PrintSummary(
    StreamWriter streamWriter, 
    IEnumerable<SMS_Report_Line> smsReportLines,
    bool omitElevationAngleSetsWithoutAStationId)
{
    var matchCount = smsReportLines.Count(_ => _.comment.Substring(0, 5) == "match");
    var noStationIDInSMSCount = smsReportLines.Count(_ => _.comment == "station id missing in sms database");
    var noElevatoinAnglesetWithGivenStationIdAndDateCount = smsReportLines.Count(_ => _.comment == "no elevation angle set for given station id and date in statdb");
    var mismatchCount = smsReportLines.Count(_ => _.comment.Substring(0, 5) == "misma");
    var sum = matchCount + noStationIDInSMSCount + noElevatoinAnglesetWithGivenStationIdAndDateCount + mismatchCount;
    var totalCount = smsReportLines.Count();

    if (sum != totalCount)
    {
        throw new InvalidDataException();
    }

    if (omitElevationAngleSetsWithoutAStationId)
    {
        totalCount -= noStationIDInSMSCount;
    }

    PrintLine(streamWriter, $"  Elevation angle sets in SMS matching an elevation angles set in StatDB:       {matchCount,10} ({matchCount * 100.0 / totalCount:N3} %)");

    if (!omitElevationAngleSetsWithoutAStationId)
    {
        PrintLine(streamWriter, $"  Elevation angle sets in SMS without a station id:                             {noStationIDInSMSCount,10} ({noStationIDInSMSCount * 100.0 / totalCount:N3} %)");
    }

    PrintLine(streamWriter, $"  Elevation angle sets in SMS that are not present in StatDB:                   {noElevatoinAnglesetWithGivenStationIdAndDateCount,10} ({noElevatoinAnglesetWithGivenStationIdAndDateCount * 100.0 / totalCount:N3} %)");
    PrintLine(streamWriter, $"  Elevation angle sets in SMS with a mismatching elevation angle set in StatDB: {mismatchCount,10} ({mismatchCount * 100.0 / totalCount:N3} %)");
    PrintLine(streamWriter, $"  Elevation angle sets in total in SMS:                                         {totalCount,10}");
}