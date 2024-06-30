using System.Text;
using Npgsql;
using MetaDataInspector.Domain.StatDB;

namespace MetaDataInspector;

public static class StatDB
{
    public static List<Station> RetrieveStations()
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

                    stations.Add(new Station
                    {
                        statid = statid,
                        icao_id = icao_id,
                        country = country,
                        source = source
                    });
                }
            }

        }
        catch (PostgresException excp)
        {
            throw excp;
        }

        return stations;
    }

    public static List<Name> RetrieveNames()
    {
        var names = new List<Name>();

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
                "station.statid, " +
                "name.start_time, " +
                "name.name " +
                "FROM station INNER JOIN name ON station.statid = name.statid"
            );

            queryBuilder.AppendLine(" WHERE station.source = 'ing'");

            using (var statdb_cmd = new NpgsqlCommand(queryBuilder.ToString(), statdb_conn))
            using (var statdb_reader = statdb_cmd.ExecuteReader())
            {
                while (statdb_reader.Read())
                {
                    int? statid = statdb_reader.IsDBNull(0) ? null : statdb_reader.GetInt32(0);
                    DateTime? start_time = statdb_reader.IsDBNull(1) ? null : statdb_reader.GetDateTime(1);
                    string? name = statdb_reader.IsDBNull(2) ? null : statdb_reader.GetString(2);

                    names.Add(new Name
                    {
                        statid = statid,
                        start_time = start_time,
                        name = name
                    });
                }
            }

        }
        catch (PostgresException excp)
        {
            throw excp;
        }

        return names;
    }
}