using System.Data;
using MigrationScriptGenerator;
using Npgsql;

static void PrintLine(
    StreamWriter streamWriter,
    string line)
{
    Console.WriteLine(line);
    streamWriter.WriteLine(line);
}

//var lines = File.ReadAllLines(@"..\..\..\data\position_tmp.SQL"); // MELO-HOME
var lines = File.ReadAllLines(@"data/position_tmp.SQL"); // Linux

var positionRowsFromNettoListe = new List<PositionRow>();

using (var streamWriter1 = new StreamWriter("output1.txt"))

    foreach (var line in lines.Skip(2))
    {
        if (string.IsNullOrEmpty(line))
        {
            continue;
        }

        var values = line.Split(',');

        var positionRow = new PositionRow
        {
            StationIdDMI = int.Parse(values[0].Trim('(')),
            Entity = values[1],
            StartString = values[2],
            EndString = values[3],
            LatitudeString = values[4],
            LongitudeString = values[5],
            HeightString = values[6].Trim(')')
        };

        if (positionRow.Entity != "'station'")
        {
            throw new InvalidDataException("Unexpected value");
        }

        positionRowsFromNettoListe.Add(positionRow);

        //var sb = new StringBuilder();
        //sb.Append($"{positionRow.StationIdDMI}");
        //sb.Append($" - {positionRow.DummyStationString}");
        //sb.Append($" - {positionRow.StartString}");
        //sb.Append($" ({positionRow.StartTime})");
        //sb.Append($" - {positionRow.EndString}");
        //sb.Append($" ({positionRow.EndTime})");
        //sb.Append($" - {positionRow.LatitudeString}");
        //sb.Append($" - {positionRow.LongitudeString}");
        //sb.Append($" - {positionRow.HeightString}");

        //var logMessage = sb.ToString();

        //PrintLine(streamWriter1, logMessage);
    }

// Her har vi læst alle positions fra nettolisten ind i en liste

var statdb_host = "nanoq.dmi.dk";
var statdb_user = "ebs";
var statdb_password = "Vm6PAkPh";
var statdb_database = "statdb";
var statdb_connectionString = $"Host={statdb_host};Username={statdb_user};Password={statdb_password};Database={statdb_database}";

// https://www.nuget.org/packages/Npgsql.NodaTime

using var statdb_conn = new NpgsqlConnection(statdb_connectionString);
statdb_conn.Open();

var positionRowsFromTargetDatabase = new List<PositionRow>();

try
{
    var query =
        "SELECT " + 
        "statid, " + 
        "entity, " + 
        "start_time, " + 
        "end_time, " + 
        "lat, " + 
        "long, " + 
        "height " + 
        "FROM public.position " +
        "WHERE entity = 'station'";

    // TEMPOPORARY
    query += " LIMIT 5";

    using (var statdb_cmd = new NpgsqlCommand(query, statdb_conn))
    using (var statdb_reader = statdb_cmd.ExecuteReader())
    using (var streamWriter = new StreamWriter("output.txt"))
    {
        while (statdb_reader.Read())
        {
            var statid = statdb_reader.GetInt32(0);
            var entity = statdb_reader.IsDBNull(1) ? null : statdb_reader.GetString(1);
            var start_time = statdb_reader.GetDateTime(2);

            var positionRow = new PositionRow
            {
                StationIdDMI = statid,
                Entity = entity == null ? "" : entity,
                // StartString = values[2],
                // EndString = values[3],
                // LatitudeString = values[4],
                // LongitudeString = values[5],
                // HeightString = values[6].Trim(')')
            };

            // var statid = statdb_reader.GetInt32(0);
            // var icao_id = statdb_reader.IsDBNull(1) ? "(null)" : statdb_reader.GetString(1);
            // var country = statdb_reader.IsDBNull(2) ? "(null)" : statdb_reader.GetString(2);
            // var source = statdb_reader.IsDBNull(3) ? "(null)" : statdb_reader.GetString(3);
            PrintLine(streamWriter, $"{statid}");
        }
    }
}
catch (PostgresException excp)
{
    throw excp;
}
