using System.Text;
using Npgsql;
using MetaDataInspector.Domain.SMS;
using MetaDataInspector.Domain.StatDB;

namespace MetaDataInspector;

public static class DataHelpers
{
    public static List<StationInformation> SMS_RetrieveStationInformations(
        bool includeSynopStations,
        bool includeSVKStations,
        bool includePluvioStations)
    {
        var stationInformations = new List<StationInformation>();

        try
        {
            var sms_host = "172.25.7.23:5432";
            var sms_user = "ebs";
            var sms_password = "Vm6PAkPh";
            var sms_database = "sms_prod";
            var sms_connectionString = $"Host={sms_host};Username={sms_user};Password={sms_password};Database={sms_database}";

            using var sms_conn = new NpgsqlConnection(sms_connectionString);
            sms_conn.Open();

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

                    int objectId = sms_reader.GetInt32(0);
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

        return stationInformations;
    }

    public static List<Station> StatDB_RetrieveStations()
    {
        var stations = new List<Station>();

        try
        {
            var statdb_host = "nanoq.dmi.dk";
            var statdb_user = "ebs";
            var statdb_password = "Vm6PAkPh";
            var statdb_database = "statdb";
            var statdb_connectionString = $"Host={statdb_host};Username={statdb_user};Password={statdb_password};Database={statdb_database}";

            using var statdb_conn = new NpgsqlConnection(statdb_connectionString);
            statdb_conn.Open();

            var queryBuilder = new StringBuilder(
                "SELECT " +
                "statid, " +
                "icao_id, " +
                "country, " +
                "source " +
                "FROM station"
            );

            queryBuilder.AppendLine(" WHERE source = 'ing'");

            using (var statdb_cmd = new NpgsqlCommand(queryBuilder.ToString(), statdb_conn))
            using (var statdb_reader = statdb_cmd.ExecuteReader())
            {
                while (statdb_reader.Read())
                {
                    int? statid = statdb_reader.IsDBNull(0) ? null : statdb_reader.GetInt32(0);
                    string? icao_id = statdb_reader.IsDBNull(1) ? null : statdb_reader.GetString(1);
                    string? country = statdb_reader.IsDBNull(2) ? null : statdb_reader.GetString(2);
                    string? source = statdb_reader.IsDBNull(3) ? null : statdb_reader.GetString(3);

                    var station = new Station
                    {
                        statid = statid,
                        icao_id = icao_id,
                        country = country,
                        source = source
                    };

                    stations.Add(station);
                }
            }

        }
        catch (PostgresException excp)
        {
            throw excp;
        }

        return stations;
    }
}