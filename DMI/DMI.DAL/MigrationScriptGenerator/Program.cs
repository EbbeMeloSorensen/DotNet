using System.Globalization;
using System.Text;
using MigrationScriptGenerator;
using Npgsql;

//var lines = File.ReadAllLines(@"..\..\..\data\position_tmp.SQL"); // MELO-HOME
var lines = File.ReadAllLines(@"data/position_tmp.SQL"); // Linux

// I: Load positions from payload
var positionRowsFromPayload = new List<PositionRow>();

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
        Latitude = double.Parse(values[4], CultureInfo.InvariantCulture),
        Longitude = double.Parse(values[5], CultureInfo.InvariantCulture),
        Height = double.Parse(values[6].Trim(')'), CultureInfo.InvariantCulture)
    };

    if (positionRow.Entity != "'station'")
    {
        throw new InvalidDataException("Unexpected value");
    }

    positionRowsFromPayload.Add(positionRow);
}

var total = positionRowsFromPayload.Count;
var distinct = positionRowsFromPayload.Select(_ => _.StationIdDMI).Distinct().Count();
if (total > distinct)
{
    throw new InvalidDataException("multiple positions for same station found");
}

// II: Load positions from database

var positionRowsFromTargetDatabase = new List<PositionRow>();

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
        "entity, " + 
        "start_time::TEXT, " + 
        "end_time::TEXT, " + 
        "lat, " + 
        "long, " + 
        "height " + 
        "FROM public.position " +
        "WHERE entity = 'station' " +
        "ORDER BY statid";

    // TEMPOPORARY
    //query += " LIMIT 100";

    using (var statdb_cmd = new NpgsqlCommand(query, statdb_conn))
    using (var statdb_reader = statdb_cmd.ExecuteReader())
    {
        while (statdb_reader.Read())
        {
            var statid = statdb_reader.GetInt32(0);
            var entity = statdb_reader.IsDBNull(1) ? null : statdb_reader.GetString(1);
            var start_time = statdb_reader.IsDBNull(2) ? "" : statdb_reader.GetString(2);
            var end_time = statdb_reader.IsDBNull(3) ? "" : statdb_reader.GetString(3);
            var latitude = statdb_reader.IsDBNull(4) ? new double() : statdb_reader.GetDouble(4);
            var longitude = statdb_reader.IsDBNull(5) ? new double() : statdb_reader.GetDouble(5);
            var height = statdb_reader.IsDBNull(6) ? new double() : statdb_reader.GetDouble(6);

            if (statid == 500520)
            {
                int a = 0;
            }

            var positionRow = new PositionRow
            {
                StationIdDMI = statid,
                Entity = entity == null ? "" : entity,
                StartString = start_time,
                EndString = end_time,
                Latitude = latitude,
                Longitude = longitude,
                Height = height
            };

            positionRowsFromTargetDatabase.Add(positionRow);
        }
    }
}
catch (PostgresException excp)
{
    throw excp;
}

// III: Traverse the positions from the payload and report clashes

var count = 0;

using (var streamWriter = new StreamWriter("output.txt"))
{
    foreach (var positionRow in positionRowsFromPayload)
    {
        streamWriter.PrintLine($"Migrating position for station {positionRow.StationIdDMI}..");

        //var timeIntervalPayload = new TimeInterval()

        var sb = new StringBuilder($"  Time interval in payload: ");
        sb.Append(positionRow.StartTime.AsShortDateString());
        sb.Append(" -> ");
        sb.Append(positionRow.EndTime.AsShortDateString());

        streamWriter.PrintLine(sb.ToString());

        positionRowsFromTargetDatabase
            .Where(_ => _.StationIdDMI == positionRow.StationIdDMI)
            .ToList()
            .ForEach(_ =>
            {
                var sb = new StringBuilder($"  Time interval in database: ");
                sb.Append(_.StartTime.AsShortDateString());
                sb.Append(" -> ");
                sb.Append(_.EndTime.AsShortDateString());

                streamWriter.PrintLine(sb.ToString());
            });

        count++;

        if (count >= 100)
        {
            break;
        }
    }
}
