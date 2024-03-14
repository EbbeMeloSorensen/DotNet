using System.Globalization;
using System.Text;
using Npgsql;

namespace MigrationScriptGenerator;

public static class Application
{
    const string _statdb_host = "nanoqt.dmi.dk";
    const string _statdb_user = "ebs";
    const string _statdb_password = "Vm6PAkPh";
    const string _statdb_database = "statdb";
    const string _statdb_connectionString = $"Host={_statdb_host};Username={_statdb_user};Password={_statdb_password};Database={_statdb_database}";

    public static List<PositionRow> LoadPositionsFromPayload(
        string file)
    {
        var result = new List<PositionRow>();
        var lines = File.ReadAllLines(file);

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

            result.Add(positionRow);
        }

        var total = result.Count;
        var distinct = result.Select(_ => _.StationIdDMI).Distinct().Count();
        if (total > distinct)
        {
            throw new InvalidDataException("multiple positions for same station found");
        }

        return result;
    }

    public static List<PositionRow> LoadPositionsFromDatabase(
        int? limit = null)
    {
        var result = new List<PositionRow>();

        using var statdb_conn = new NpgsqlConnection(_statdb_connectionString);
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

            if (limit.HasValue)
            {
                query += $" LIMIT {limit.Value}";
            }

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

                    result.Add(positionRow);
                }
            }
        }
        catch (PostgresException excp)
        {
            throw excp;
        }

        return result;
    }

    public static void GenerateSQLScriptForPositionTable(
        List<PositionRow> positionRowsFromPayload,
        List<PositionRow> positionRowsFromTargetDatabase,
        int? limit = 0)
    {
        var count = 0;

        using var scriptWriter = new StreamWriter("manipulate_positions.sql");
        using var verboseLogWriter = new StreamWriter("log_verbose.txt");
        using var briefLogWriter = new StreamWriter("log_brief.txt");

        foreach (var positionRow in positionRowsFromPayload)
        {
            var includeInBriefLog = false;
            var potentialMessagesForBriefLog = new List<string>();

            var logMessage = $"Inspecting existing positions of station {positionRow.StationIdDMI}..";

            verboseLogWriter.PrintLine(logMessage, true);
            potentialMessagesForBriefLog.Add(logMessage);

            var timeIntervalPayload = new TimeInterval
            {
                Start = positionRow.StartTime,
                End = positionRow.EndTime
            };

            positionRowsFromTargetDatabase
                .Where(_ => _.StationIdDMI == positionRow.StationIdDMI)
                .OrderBy(_ => _.StartTime)
                .ToList()
                .ForEach(_ =>
                {
                    var timeIntervalDatabase = new TimeInterval
                    {
                        Start = _.StartTime,
                        End = _.EndTime
                    };

                    logMessage = $"  Time interval in database: {timeIntervalDatabase}";

                    if (timeIntervalDatabase.CoveredBy(timeIntervalPayload))
                    {
                        var whereClauseBuilder = new StringBuilder();
                        whereClauseBuilder.Append($"statid = {_.StationIdDMI} AND ");
                        whereClauseBuilder.Append($"start_time = '{_.StartString}' AND ");
                        whereClauseBuilder.Append($"end_time = '{_.EndString}'");
                        var whereClause = whereClauseBuilder.ToString();

                        VerifyThatWhereClauseCorrespondsToOneRow(whereClause);

                        var deltaLatitude = positionRow.Latitude - _.Latitude;
                        var deltaLongitude = positionRow.Longitude - _.Longitude;

                        logMessage += $" (covered                                                         (delta_lat, delta_long) = ({deltaLatitude:N6}, {deltaLongitude:N6}))";
                        includeInBriefLog = false;
                    }
                    else if (timeIntervalDatabase.Overlaps(timeIntervalPayload))
                    {
                        var trimmedTimeInterval = timeIntervalDatabase.Trim(timeIntervalPayload);

                        var startTimeExistingRow = "2018-12-04 10:31:55.000";

                        var whereClauseBuilder = new StringBuilder();
                        whereClauseBuilder.Append($"statid = {_.StationIdDMI} AND ");
                        whereClauseBuilder.Append($"start_time = '{_.StartString}' AND ");
                        whereClauseBuilder.Append($"end_time = '{_.EndString}'");
                        var whereClause = whereClauseBuilder.ToString();

                        VerifyThatWhereClauseCorrespondsToOneRow(whereClause);

                        var sqlStatementBuilder = new StringBuilder("UPDATE position SET ");

                        // var temp1 = timeIntervalDatabase.Start.AsShortDateString();
                        // var temp2 = trimmedTimeInterval.Start.AsShortDateString();

                        // if (trimmedTimeInterval.Start.AsShortDateString() !=
                        //     timeIntervalDatabase.Start.AsShortDateString())
                        // {
                        // }


                        sqlStatementBuilder.Append($"end_time = '{startTimeExistingRow}'");
                        sqlStatementBuilder.Append(" WHERE ");
                        sqlStatementBuilder.Append($"{whereClause};");

                        var sqlStatement = sqlStatementBuilder.ToString();
                        scriptWriter.PrintLine(sqlStatement, false);

                        logMessage += $" (overlap) -> {trimmedTimeInterval}";
                        includeInBriefLog = true;
                    }
                    else
                    {
                        logMessage += $"           -> {timeIntervalDatabase}";
                    }

                    verboseLogWriter.PrintLine(logMessage, true);
                    potentialMessagesForBriefLog.Add(logMessage);
                });

            logMessage = $"  Time interval in payload:  {timeIntervalPayload}           -> {timeIntervalPayload}";
            verboseLogWriter.PrintLine(logMessage, true);
            potentialMessagesForBriefLog.Add(logMessage);

            logMessage = "-----------------------------------------------------------------------------------------------";
            verboseLogWriter.PrintLine(logMessage, true);
            potentialMessagesForBriefLog.Add(logMessage);

            if (includeInBriefLog)
            {
                foreach (var message in potentialMessagesForBriefLog)
                {
                    briefLogWriter.PrintLine(message, false);
                }
            }

            count++;

            if (limit.HasValue && count >= limit.Value)
            {
                break;
            }
        }        
    }

    private static void VerifyThatWhereClauseCorrespondsToOneRow(
        string whereClause)
    {
        var verificationQuery = $"SELECT COUNT(statid) FROM position WHERE {whereClause};";

        using var statdb_conn = new NpgsqlConnection(_statdb_connectionString);
        statdb_conn.Open();
        using (var statdb_cmd = new NpgsqlCommand(verificationQuery, statdb_conn))
        {
            var rowCount = statdb_cmd.ExecuteScalar();

            if (rowCount == null || rowCount.ToString() != "1")
            {
                throw new InvalidOperationException("Unexpected row count");
            }
        }
    }
}