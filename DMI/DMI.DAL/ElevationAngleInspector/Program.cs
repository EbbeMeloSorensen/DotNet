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
        "sde.stationinformation.stationid_dmi, " + 
        "sde.stationinformation.stationname, " + 
        "sde.stationinformation.wgs_lat, " +
        "sde.stationinformation.wgs_long, " + 
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
        "FROM sde.stationinformation " + 
        "INNER JOIN sde.sensorlocation ON sde.stationinformation.stationid_dmi=sde.sensorlocation.stationid_dmi " + 
        "INNER JOIN sde.elevationangles ON sde.sensorlocation.globalid=sde.elevationangles.parentguid " + 
        "WHERE sde.stationinformation.gdb_to_date = '9999-12-31 23:59:59' " + 
        "AND sde.elevationangles.gdb_to_date = '9999-12-31 23:59:59' " + 
        "ORDER BY sde.stationinformation.stationid_dmi, sde.elevationangles.datefrom " +
        "--LIMIT 10";

    using (var sms_cmd = new NpgsqlCommand(sms_query, sms_conn))
    using (var sms_reader = sms_cmd.ExecuteReader())
    {
        while (sms_reader.Read())
        {
            if (!sms_reader.IsDBNull(0))
            {
                sms_stations.Add(new SMS_Station
                {
                    stationid_dmi = sms_reader.GetInt32(0),
                    stationname = sms_reader.GetString(1),
                    wgs_lat = sms_reader.IsDBNull(2) ? null : sms_reader.GetDouble(2),
                    wgs_long = sms_reader.IsDBNull(3) ? null : sms_reader.GetDouble(3),
                    datefrom = sms_reader.GetDateTime(4),
                    angle_n = sms_reader.GetInt32(5),
                    angle_ne = sms_reader.GetInt32(6),
                    angle_e = sms_reader.GetInt32(7),
                    angle_se = sms_reader.GetInt32(8),
                    angle_s = sms_reader.GetInt32(9),
                    angle_sw = sms_reader.GetInt32(10),
                    angle_w = sms_reader.GetInt32(11),
                    angle_nw = sms_reader.GetInt32(12),
                    angleindex = sms_reader.GetInt32(13)
                });
            }
        }
    }
}
catch (PostgresException excp)
{
    throw excp;
}

var statdb_host = "nanoq.dmi.dk";
var statdb_user = "ebs";
var statdb_password = "secret";
var statdb_database = "statdb";
var statdb_connectionString = $"Host={statdb_host};Username={statdb_user};Password={statdb_password};Database={statdb_database}";

using (var streamWriter = new StreamWriter("ElevationAngles_nanoq3.txt"))
{
    foreach(var sms_station in sms_stations)
    {
        using var statdb_conn = new NpgsqlConnection(statdb_connectionString);
        statdb_conn.Open();

        var datefrom = sms_station.datefromAsDBString();

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
            $"WHERE CAST(station.statid as VARCHAR) LIKE '{sms_station.stationid_dmi}%' " +
            $"AND leeindex.start_time = '{datefrom}'";

        var line = sms_station.ToString();

        var compareResult = "NOT PRESENT IN STATDB!!";
        string statdb_station_as_string = "";

        using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
        using (var statdb_reader = statdb_cmd.ExecuteReader())
        {
            if (statdb_reader.Read())
            {
                if (!statdb_reader.IsDBNull(0))
                {
                    var statdb_station = new STATDB_Station
                    {
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
                        leeindexindex = statdb_reader.GetInt32(10),
                    };

                    if (statdb_station.leeindex_n == sms_station.angle_n &&
                        statdb_station.leeindex_ne == sms_station.angle_ne &&
                        statdb_station.leeindex_e == sms_station.angle_e &&
                        statdb_station.leeindex_se == sms_station.angle_se &&
                        statdb_station.leeindex_s == sms_station.angle_s &&
                        statdb_station.leeindex_sw == sms_station.angle_sw &&
                        statdb_station.leeindex_w == sms_station.angle_w &&
                        statdb_station.leeindex_nw == sms_station.angle_nw &&
                        statdb_station.leeindexindex == sms_station.angleindex)
                    {
                        compareResult = "match";                        
                    }
                    else
                    {
                        compareResult = "MISMATCH BETWEEN SMS AND STATDB!!";
                        statdb_station_as_string = statdb_station.ToString();
                    }
                }
            }
        }

        line += $"  {compareResult}";

        if (compareResult == "MISMATCH BETWEEN SMS AND STATDB!!")
        {
            line += $"  (row in statdb: {statdb_station_as_string})";
        }

        Console.WriteLine(line);
        streamWriter.WriteLine(line);
    }
}


