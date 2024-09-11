using System.Text;
using Npgsql;

namespace MetaDataInspector;

public static class ObsDB
{
    public static void RetrieveObservations(
        int year,
        string parameter,
        int? limit = null)
    {
        try
        {
            var obsdb_host = "nanoq.dmi.dk";
            var obsdb_user = "ebs";
            var obsdb_password = "Vm6PAkPh";
            var obsdb_database = "obsdb";
            var obsdb_connectionString = $"Host={obsdb_host};Username={obsdb_user};Password={obsdb_password};Database={obsdb_database}";

            using var obsdb_conn = new NpgsqlConnection(obsdb_connectionString);
            obsdb_conn.Open();

            var queryBuilder = new StringBuilder(
                "SELECT " +
                "statid, " +
                "timeobs, " +
                $"{parameter} " +
                $"FROM precip_hum_pressure_{year} " +
                $"WHERE best = true AND {parameter} IS NOT NULL " +
                "ORDER BY statid, timeobs"
            );

            if (limit != null)
            {
                queryBuilder.Append($" LIMIT {limit.Value}");
            }

            queryBuilder.Append(";");

            var query = queryBuilder.ToString();
            var fileName = $"{parameter}_{year}.csv";

            if (!File.Exists(fileName))
            {
                using (var streamWriter = new StreamWriter(fileName))
                using (var obsdb_cmd = new NpgsqlCommand(query, obsdb_conn))
                using (var obsdb_reader = obsdb_cmd.ExecuteReader())
                {
                    while (obsdb_reader.Read())
                    {
                        var statid = obsdb_reader.GetInt32(0);
                        var timeobs = obsdb_reader.GetDateTime(1);
                        var value = obsdb_reader.GetDouble(2);

                        streamWriter.WriteLine($"{statid},{timeobs.AsDateTimeString(false, false)},{value:F5}");
                    }
                }
            }
        }
        catch (PostgresException excp)
        {
            throw excp;
        }
    }
}