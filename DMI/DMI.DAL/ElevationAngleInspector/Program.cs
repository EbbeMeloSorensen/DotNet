using Npgsql;

var host = "";
var user = "";
var password = "";
var database = "";
var connectionString = $"Host={host};Username={user};Password={password};Database={database}";

using var conn = new NpgsqlConnection(connectionString);
conn.Open();

try
{
    var query = "SELECT statid FROM station";

    using (var cmd = new NpgsqlCommand(query, conn))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            if (!reader.IsDBNull(0))
            {
                var statid = reader.GetInt32(0);

                //result.Add(statid);
                Console.WriteLine("hej");
            }
        }
    }
}
catch (PostgresException excp)
{
    throw excp;
}
