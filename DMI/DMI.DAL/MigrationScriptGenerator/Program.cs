using MigrationScriptGenerator;
using Npgsql;

var statdb_host = "nanoq.dmi.dk";
var statdb_user = "ebs";
var statdb_password = "Vm6PAkPh";
var statdb_database = "statdb";
var statdb_connectionString = $"Host={statdb_host};Username={statdb_user};Password={statdb_password};Database={statdb_database}";

using var statdb_conn = new NpgsqlConnection(statdb_connectionString);
statdb_conn.Open();

try
{
    var query =
        "SELECT " + 
        "statid, " + 
        "icao_id, " + 
        "country, " + 
        "source " + 
        "FROM public.station " +
        "LIMIT 3";

    using (var statdb_cmd = new NpgsqlCommand(query, statdb_conn))
    using (var statdb_reader = statdb_cmd.ExecuteReader())
    using (var streamWriter = new StreamWriter("output.txt"))
    {
        while (statdb_reader.Read())
        {
            var statid = statdb_reader.GetInt32(0);
            var icao_id = statdb_reader.IsDBNull(1) ? "(null)" : statdb_reader.GetString(1);
            var country = statdb_reader.IsDBNull(2) ? "(null)" : statdb_reader.GetString(2);
            var source = statdb_reader.IsDBNull(3) ? "(null)" : statdb_reader.GetString(3);
            PrintLine(streamWriter, $"{statid}, {icao_id}, {country}, {source}");
        }
    }
}
catch (PostgresException excp)
{
    throw excp;
}

static void PrintLine(
    StreamWriter streamWriter, 
    string line)
{
    Console.WriteLine(line);
    streamWriter.WriteLine(line);
}
