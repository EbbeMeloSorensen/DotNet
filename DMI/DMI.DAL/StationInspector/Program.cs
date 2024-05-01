using System.Text;
using Npgsql;
using StationInspector;

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
        "stationowner " +
        "FROM sde.stationinformation " +
        "WHERE gdb_to_date = '9999-12-31 23:59:59.000000'" +
        //" AND stationtype IN (0, 5)" +
        " AND stationtype IN (5)" +
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

            var stationInformation = new StationInformation
            {
                objectid = objectId,
                stationname = stationname,
                stationid_dmi = stationid_dmi,
                stationtype = stationtype,
                accessaddress = accessaddress,
                country = country,
                status = status
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

using (var streamWriter = new StreamWriter("output.txt"))
foreach (var stationInformation in stationInformations)
{
    var stationNameAsString = stationInformation.stationname != null ? $"\"{stationInformation.stationname}\"" : "<null>";
    var stationIdDMIAsString = stationInformation.stationid_dmi.HasValue ? $"{stationInformation.stationid_dmi.Value}" : "<null>";
    var stationTypeAsString = stationInformation.stationtype.HasValue ? $"{stationInformation.stationtype.Value}" : "<null>";
    var accessaddressAsString = stationInformation.accessaddress != null ? stationInformation.accessaddress : "<null>";
    var countryAsString = stationInformation.country.HasValue ? $"{stationInformation.country.Value}" : "<null>";
    var statusAsString = stationInformation.status.HasValue ? $"{stationInformation.status.Value}" : "<null>";

    var sb = new StringBuilder();
    //sb.Append($"{rowCount, 4}: ");
    sb.Append($"{stationNameAsString, -30}");
    sb.Append($"{stationIdDMIAsString, 5}");
    sb.Append($"{stationTypeAsString, 5} ");
    //sb.Append($"{accessaddressAsString, -120}");
    sb.Append($"{countryAsString, -5}");
    sb.Append($"{statusAsString, -5}");

    PrintLine(streamWriter, sb.ToString());

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
            PrintLine(streamWriter,  $"No Matching station in statdb");
        }
        else
        {
            //PrintLine(streamWriter,  $"Matching station in statdb: ");
        }

        statdb_query = 
            "SELECT " + 
            "statid, " + 
            "icao_id, " + 
            "country, " + 
            "source " + 
            "FROM station " + 
            $"WHERE statid::TEXT LIKE('{stationInformation.stationid_dmi}__');";

        using (var statdb_cmd = new NpgsqlCommand(statdb_query, statdb_conn))
        using (var statdb_reader = statdb_cmd.ExecuteReader())
        {
        }

        PrintLine(streamWriter, "---------------------------------------------------------------");
    }
    catch (PostgresException excp)
    {
        throw excp;
    }
}

static void PrintLine(
    StreamWriter streamWriter, 
    string line)
{
    Console.WriteLine(line);
    streamWriter.WriteLine(line);
}
