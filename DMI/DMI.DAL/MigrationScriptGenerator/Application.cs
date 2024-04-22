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

    public static List<LeeIndexRow> LoadLeeIndexesFromPayload(
        string file)
    {
        var result = new List<LeeIndexRow>();
        var lines = File.ReadAllLines(file);

        foreach (var line in lines.Skip(2))
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var values = line.Split(',');

            // var positionRow = new PositionRow
            // {
            //     StationIdDMI = int.Parse(values[0].Trim('(')),
            //     Entity = values[1],
            //     StartString = values[2],
            //     EndString = values[3],
            //     Latitude = double.Parse(values[4], CultureInfo.InvariantCulture),
            //     Longitude = double.Parse(values[5], CultureInfo.InvariantCulture),
            //     Height = double.Parse(values[6].Trim(')'), CultureInfo.InvariantCulture)
            // };

            var leeIndexRow = new LeeIndexRow
            {
                StationIdDMI = int.Parse(values[0].Trim('(')),
                StartString = values[1],
                EndString = values[2],
                S = double.Parse(values[3], CultureInfo.InvariantCulture),
                SW = double.Parse(values[4], CultureInfo.InvariantCulture),
                W = double.Parse(values[5], CultureInfo.InvariantCulture),
                NW = double.Parse(values[6], CultureInfo.InvariantCulture),
                N = double.Parse(values[7], CultureInfo.InvariantCulture),
                NE = double.Parse(values[8], CultureInfo.InvariantCulture),
                E = double.Parse(values[9], CultureInfo.InvariantCulture),
                SE = double.Parse(values[10], CultureInfo.InvariantCulture),
                Index = double.Parse(values[11], CultureInfo.InvariantCulture),
                Comment = values[12].Trim(')')
            };

            if (leeIndexRow.Comment != "'source: SMS.'")
            {
                throw new InvalidDataException("Unexpected value");
            }

            result.Add(leeIndexRow);
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

    public static List<LeeIndexRow> LoadLeeIndexesFromDatabase(
        int? limit = null)
    {
        var result = new List<LeeIndexRow>();

        using var statdb_conn = new NpgsqlConnection(_statdb_connectionString);
        statdb_conn.Open();

        try
        {
            var query =
                "SELECT " + 
                "statid, " + 
                "start_time::TEXT, " + 
                "end_time::TEXT, " + 
                "s, " + 
                "sw, " + 
                "w, " + 
                "nw, " + 
                "n, " + 
                "ne, " + 
                "e, " + 
                "se, " + 
                "index, " + 
                "comment " + 
                "FROM public.leeindex2 " +
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
                    var start_time = statdb_reader.IsDBNull(1) ? "" : statdb_reader.GetString(1);
                    var end_time = statdb_reader.IsDBNull(2) ? "" : statdb_reader.GetString(2);
                    var s = statdb_reader.GetInt32(3);
                    var sw = statdb_reader.GetInt32(4);
                    var w = statdb_reader.GetInt32(5);
                    var nw = statdb_reader.GetInt32(6);
                    var n = statdb_reader.GetInt32(7);
                    var ne = statdb_reader.GetInt32(8);
                    var e = statdb_reader.GetInt32(9);
                    var se = statdb_reader.GetInt32(10);
                    var index = statdb_reader.GetInt32(11);
                    var comment = statdb_reader.IsDBNull(12) ? null : statdb_reader.GetString(12);

                    var leeIndexRow = new LeeIndexRow
                    {
                        StationIdDMI = statid,
                        StartString = start_time,
                        EndString = end_time,
                        S = s,
                        SW = sw,
                        W = w,
                        NW = nw,
                        N = n,
                        NE = ne,
                        E = e,
                        SE = se,
                        Index = index,
                        Comment = comment
                    };

                    result.Add(leeIndexRow);
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

                        var sqlStatementBuilder = new StringBuilder("DELETE position");
                        sqlStatementBuilder.Append(" WHERE ");
                        sqlStatementBuilder.Append($"{whereClause};");
                        var sqlStatement = sqlStatementBuilder.ToString();
                        scriptWriter.PrintLine(sqlStatement, false);

                        var deltaLatitude = positionRow.Latitude - _.Latitude;
                        var deltaLongitude = positionRow.Longitude - _.Longitude;
                        logMessage += $" (covered)             -> [              (row will be deleted)               ] (delta_lat, delta_long) = ({deltaLatitude:N6}, {deltaLongitude:N6}))";
                        includeInBriefLog = true;
                    }
                    else if (timeIntervalDatabase.Overlaps(timeIntervalPayload))
                    {
                        var trimmedTimeInterval = timeIntervalDatabase.Trim(timeIntervalPayload);

                        var durationInDays = trimmedTimeInterval.Duration.TotalDays;

                        if (durationInDays < 2)
                        {
                            var whereClauseBuilder = new StringBuilder();
                            whereClauseBuilder.Append($"statid = {_.StationIdDMI} AND ");
                            whereClauseBuilder.Append($"start_time = '{_.StartString}' AND ");
                            whereClauseBuilder.Append($"end_time = '{_.EndString}'");
                            var whereClause = whereClauseBuilder.ToString();

                            VerifyThatWhereClauseCorrespondsToOneRow(whereClause);

                            var sqlStatementBuilder = new StringBuilder("DELETE position");
                            sqlStatementBuilder.Append(" WHERE ");
                            sqlStatementBuilder.Append($"{whereClause};");
                            var sqlStatement = sqlStatementBuilder.ToString();
                            scriptWriter.PrintLine(sqlStatement, false);

                            var deltaLatitude = positionRow.Latitude - _.Latitude;
                            var deltaLongitude = positionRow.Longitude - _.Longitude;
                            logMessage += $" (effectively covered) -> [              (row will be deleted)               ] (delta_lat, delta_long) = ({deltaLatitude:N6}, {deltaLongitude:N6}))";
                            includeInBriefLog = true;
                        }
                        else
                        {
                            var whereClauseBuilder = new StringBuilder();
                            whereClauseBuilder.Append($"statid = {_.StationIdDMI} AND ");
                            whereClauseBuilder.Append($"start_time = '{_.StartString}' AND ");
                            whereClauseBuilder.Append($"end_time = '{_.EndString}'");
                            var whereClause = whereClauseBuilder.ToString();

                            VerifyThatWhereClauseCorrespondsToOneRow(whereClause);

                            var sqlStatementBuilder = new StringBuilder("UPDATE position SET ");

                            var fieldUpdates = new List<string>();

                            var trimmedIntervalStart = trimmedTimeInterval.Start.AsDateTimeString();
                            var trimmedIntervalEnd = trimmedTimeInterval.End.AsDateTimeString();
                            var databaseIntervalStart = timeIntervalDatabase.Start.AsDateTimeString();
                            var databaseIntervalEnd = timeIntervalDatabase.End.AsDateTimeString();

                            if (trimmedIntervalStart != databaseIntervalStart)
                            {
                                fieldUpdates.Add($"start_time = '{trimmedTimeInterval.Start.AsDateTimeString()}'");
                            }

                            if (trimmedIntervalEnd != databaseIntervalEnd)
                            {
                                fieldUpdates.Add($"end_time = '{trimmedTimeInterval.End.AsDateTimeString()}'");
                            }

                            if (fieldUpdates.Count == 0)
                            {
                                throw new InvalidOperationException("something wrong");
                            }

                            sqlStatementBuilder.Append(fieldUpdates.Aggregate((c, n) => $"{c}, {n}"));
                            sqlStatementBuilder.Append(" WHERE ");
                            sqlStatementBuilder.Append($"{whereClause};");

                            var sqlStatement = sqlStatementBuilder.ToString();
                            scriptWriter.PrintLine(sqlStatement, false);

                            logMessage += $" (overlap)             -> {trimmedTimeInterval}";
                            includeInBriefLog = true;
                        }
                    }
                    else
                    {
                        logMessage += $"                       -> {timeIntervalDatabase}";
                    }

                    verboseLogWriter.PrintLine(logMessage, true);
                    potentialMessagesForBriefLog.Add(logMessage);
                });

            logMessage = $"  Time interval in payload:  {timeIntervalPayload}                       -> {timeIntervalPayload}";
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

    public static void GenerateSQLScriptForLeeIndexTable(
        List<LeeIndexRow> leeIndexRowsFromPayload,
        List<LeeIndexRow> leeIndexRowsFromTargetDatabase,
        int? limit = 0)
    {
        var count = 0;

        using var scriptWriter = new StreamWriter("manipulate_leeindexes.sql");
        using var verboseLogWriter = new StreamWriter("log_verbose_leeindex.txt");
        using var briefLogWriter = new StreamWriter("log_brief_leeindex.txt");

        foreach (var leeIndexRow in leeIndexRowsFromPayload)
        {
            var includeInBriefLog = false;
            var potentialMessagesForBriefLog = new List<string>();

            var logMessage = $"Inspecting existing lee indexes of station {leeIndexRow.StationIdDMI}..";

            verboseLogWriter.PrintLine(logMessage, true);
            potentialMessagesForBriefLog.Add(logMessage);

            var timeIntervalPayload = new TimeInterval
            {
                Start = leeIndexRow.StartTime,
                End = leeIndexRow.EndTime
            };

            leeIndexRowsFromTargetDatabase
                .Where(_ => _.StationIdDMI == leeIndexRow.StationIdDMI)
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

                        /*
                        var sqlStatementBuilder = new StringBuilder("DELETE position");
                        sqlStatementBuilder.Append(" WHERE ");
                        sqlStatementBuilder.Append($"{whereClause};");
                        var sqlStatement = sqlStatementBuilder.ToString();
                        scriptWriter.PrintLine(sqlStatement, false);

                        var deltaLatitude = positionRow.Latitude - _.Latitude;
                        var deltaLongitude = positionRow.Longitude - _.Longitude;
                        logMessage += $" (covered)             -> [              (row will be deleted)               ] (delta_lat, delta_long) = ({deltaLatitude:N6}, {deltaLongitude:N6}))";
                        includeInBriefLog = true;
                        */
                    }
                    else if (timeIntervalDatabase.Overlaps(timeIntervalPayload))
                    {
                        var trimmedTimeInterval = timeIntervalDatabase.Trim(timeIntervalPayload);

                        var durationInDays = trimmedTimeInterval.Duration.TotalDays;

                        if (durationInDays < 2)
                        {
                            var whereClauseBuilder = new StringBuilder();
                            whereClauseBuilder.Append($"statid = {_.StationIdDMI} AND ");
                            whereClauseBuilder.Append($"start_time = '{_.StartString}' AND ");
                            whereClauseBuilder.Append($"end_time = '{_.EndString}'");
                            var whereClause = whereClauseBuilder.ToString();

                            VerifyThatWhereClauseCorrespondsToOneRow(whereClause);

                            /*
                            var sqlStatementBuilder = new StringBuilder("DELETE position");
                            sqlStatementBuilder.Append(" WHERE ");
                            sqlStatementBuilder.Append($"{whereClause};");
                            var sqlStatement = sqlStatementBuilder.ToString();
                            scriptWriter.PrintLine(sqlStatement, false);

                            var deltaLatitude = positionRow.Latitude - _.Latitude;
                            var deltaLongitude = positionRow.Longitude - _.Longitude;
                            logMessage += $" (effectively covered) -> [              (row will be deleted)               ] (delta_lat, delta_long) = ({deltaLatitude:N6}, {deltaLongitude:N6}))";
                            includeInBriefLog = true;
                            */
                        }
                        else
                        {
                            var whereClauseBuilder = new StringBuilder();
                            whereClauseBuilder.Append($"statid = {_.StationIdDMI} AND ");
                            whereClauseBuilder.Append($"start_time = '{_.StartString}' AND ");
                            whereClauseBuilder.Append($"end_time = '{_.EndString}'");
                            var whereClause = whereClauseBuilder.ToString();

                            VerifyThatWhereClauseCorrespondsToOneRow(whereClause);

                            var sqlStatementBuilder = new StringBuilder("UPDATE position SET ");

                            var fieldUpdates = new List<string>();

                            var trimmedIntervalStart = trimmedTimeInterval.Start.AsDateTimeString();
                            var trimmedIntervalEnd = trimmedTimeInterval.End.AsDateTimeString();
                            var databaseIntervalStart = timeIntervalDatabase.Start.AsDateTimeString();
                            var databaseIntervalEnd = timeIntervalDatabase.End.AsDateTimeString();

                            if (trimmedIntervalStart != databaseIntervalStart)
                            {
                                fieldUpdates.Add($"start_time = '{trimmedTimeInterval.Start.AsDateTimeString()}'");
                            }

                            if (trimmedIntervalEnd != databaseIntervalEnd)
                            {
                                fieldUpdates.Add($"end_time = '{trimmedTimeInterval.End.AsDateTimeString()}'");
                            }

                            if (fieldUpdates.Count == 0)
                            {
                                throw new InvalidOperationException("something wrong");
                            }

                            sqlStatementBuilder.Append(fieldUpdates.Aggregate((c, n) => $"{c}, {n}"));
                            sqlStatementBuilder.Append(" WHERE ");
                            sqlStatementBuilder.Append($"{whereClause};");

                            var sqlStatement = sqlStatementBuilder.ToString();
                            scriptWriter.PrintLine(sqlStatement, false);

                            logMessage += $" (overlap)             -> {trimmedTimeInterval}";
                            includeInBriefLog = true;
                        }
                    }
                    else
                    {
                        logMessage += $"                       -> {timeIntervalDatabase}";
                    }

                    verboseLogWriter.PrintLine(logMessage, true);
                    potentialMessagesForBriefLog.Add(logMessage);
                });

            logMessage = $"  Time interval in payload:  {timeIntervalPayload}                       -> {timeIntervalPayload}";
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