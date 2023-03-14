using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using Npgsql;

namespace DMI.DAL.SeaDB
{
    public class DataProvider : DataProviderBase
    {
        private readonly List<string> _basis_table_names;
        private Dictionary<string, List<string>> _columnNameMap;

        public DataProvider(
            ILogger logger) : base(logger)
        {
            _basis_table_names = GenerateBasisTableNames();
        }

        public void Initialize(
            IEnumerable<string> parametersOfInterest)
        {
            _columnNameMap = GenerateColumnNameMap(parametersOfInterest);
        }

        public Dictionary<string, int> GenerateEmptyDictionaryForStation(
            string stationId)
        {
            var aggregatedResult = new Dictionary<string, int>();

            _basis_table_names.ForEach(btn =>
            {
                var result = GenerateEmptyDictionaryCoveredByBasisTableForStation(
                    btn, stationId);

                aggregatedResult.Aggregate(result);
            });

            return aggregatedResult;
        }

        public async Task<Dictionary<string, int>> CountAllObservationsForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId,
            DateTime startTime,
            DateTime endTime,
            bool includeZeroCountParams)
        {
            return await Task.Run(() =>
            {
                var aggregatedResult = new Dictionary<string, int>();

                _basis_table_names.ForEach(btn =>
                {
                    var result = CountObservationsCoveredByBasisTableForStation(
                        host, database, obsDBUser, obsDBPassword,
                        btn, stationId, startTime, endTime, includeZeroCountParams);

                    aggregatedResult.Aggregate(result);
                });

                return aggregatedResult;
            });
        }

        private Dictionary<string, int> GenerateEmptyDictionaryCoveredByBasisTableForStation(
            string baseTableName,
            string stationId)
        {
            if (_columnNameMap == null)
            {
                throw new InvalidOperationException("Please call DataProvider.Initialize before calling other methods");
            }

            var result = new Dictionary<string, int>();

            _columnNameMap[baseTableName].ForEach(paramId =>
            {
                result[paramId] = 0;
            });

            return result;
        }

        public Dictionary<string, int> CountObservationsCoveredByBasisTableForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string baseTableName,
            string stationId,
            DateTime startTime,
            DateTime endTime,
            bool includeZeroCountParams)
        {
            if (_columnNameMap == null)
            {
                throw new InvalidOperationException("Please call DataProvider.Initialize before calling other methods");
            }

            endTime -= new TimeSpan(0, 0, 1);

            if (endTime.Year != startTime.Year)
            {
                throw new InvalidOperationException("The time interval cannot span multiple years");
            }

            var result = new Dictionary<string, int>();

            if (includeZeroCountParams)
            {
                _columnNameMap[baseTableName].ForEach(paramId =>
                {
                    result[paramId] = 0;
                });
            }

            var columnNamesForQuery = GenerateSeaLevColumnNamesForQuery(baseTableName, startTime.Year);

            if (columnNamesForQuery.Count == 0)
            {
                return result;
            }

            using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
            {
                conn.Open();

                try
                {
                    var tableName = baseTableName == "sea_temp_salt"
                        ? baseTableName
                        : GenerateSeaLevTableName(startTime.Year);

                    var query = string.Concat(
                        "SELECT ",
                        columnNamesForQuery
                            .Select(columnName => $"sum(case when {columnName} is null then 0 else 1 end) as {columnName}")
                            .Aggregate((c, n) => $"{c}, {n}"),
                        $" FROM {tableName} ",
                        $"WHERE statid = {stationId.AsSeaDBStationId()} ",
                        $"AND timeObs between '{startTime.AsString()}' and '{endTime.AsString()}'");

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            columnNamesForQuery
                                .Select((c, i) => new { Index = i, ColumnName = c })
                                .ToList()
                                .ForEach(x =>
                                {
                                    if (!reader.IsDBNull(x.Index))
                                    {
                                        var count = reader.GetInt32(x.Index);

                                        if (count > 0)
                                        {
                                            result[x.ColumnName] = count;
                                        }
                                    }
                                });
                        }
                    }
                }
                catch (PostgresException excp)
                {
                    if (!excp.Message.Contains("does not exist"))
                    {
                        throw excp;
                    }
                }
            }

            return result;
        }

        public async Task<DateTime?> IdentifyTimeForOldestObservationForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId,
            bool maximumInsteadOfMinimum)
        {
            return await Task.Run(() =>
            {
                if (_columnNameMap == null)
                {
                    throw new InvalidOperationException("Please call DataProvider.Initialize before calling other methods");
                }

                DateTime? oldestDateTime = null;

                var sealevTableNames = new List<string>
                {
                    "sealev_1878_69",
                    "sealev_1970_79",
                    "sealev_1980_89",
                    "sealev_1990_99",
                    "sealev_2000_09",
                    "sealev_2010_19",
                    "sealev_2020_29"
                };

                if (maximumInsteadOfMinimum)
                {
                    sealevTableNames.Reverse();
                }

                var tableNames = new List<string>
                {
                    "sea_temp_salt"
                };

                tableNames = tableNames.Concat(sealevTableNames).ToList();

                tableNames.ForEach(tn =>
                {
                    var timeOfOldestObservationForTable = IdentifyTimeForOldestObservationCoveredByTableForStation(
                        host, database, obsDBUser, obsDBPassword, tn, stationId, maximumInsteadOfMinimum);

                    if (timeOfOldestObservationForTable.HasValue)
                    {
                        if (!oldestDateTime.HasValue ||
                            (!maximumInsteadOfMinimum && timeOfOldestObservationForTable.Value < oldestDateTime.Value) ||
                            (maximumInsteadOfMinimum && timeOfOldestObservationForTable.Value > oldestDateTime.Value))
                        {
                            oldestDateTime = timeOfOldestObservationForTable;

                            if (tn.Contains("sealev"))
                            {
                                return;
                            }
                        }
                    }
                });

                return oldestDateTime;
            });
        }

        public DateTime? IdentifyTimeForOldestObservationCoveredByTableForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string tableName,
            string stationId,
            bool maximumInsteadOfMinimum)
        {
            if (_columnNameMap == null)
            {
                throw new InvalidOperationException("Please call DataProvider.Initialize before calling other methods");
            }

            DateTime? result = null;

            using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
            {
                try
                {
                    conn.Open();

                    var query = string.Concat(
                        "SELECT timeObs",
                        $" FROM {tableName}",
                        $" WHERE statid = {stationId.AsSeaDBStationId()}",
                        //" AND status_ok = 1", // nogle af tabellerne har ikke denne kolonne
                        " ORDER BY timeObs",
                        maximumInsteadOfMinimum ? " DESC" : " ASC",
                        " LIMIT 1");

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = reader.GetDateTime(0);
                        }
                    }
                }
                catch (PostgresException excp)
                {
                    if (!excp.Message.Contains("does not exist"))
                    {
                        throw excp;
                    }
                }
                catch (System.Net.Sockets.SocketException excp)
                {
                    // Dette kan ske over en svag forbindelse som f.eks. wifi hjemmefra
                    throw excp;
                }
            }

            return result;
        }

        public Dictionary<string, List<Tuple<DateTime, float>>> RetrieveObservationsCoveredByBasisTableForStationInGivenYear(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string baseTableName,
            string stationId,
            int year)
        {
            if (_columnNameMap == null)
            {
                throw new InvalidOperationException("Please call DataProvider.Initialize before calling other methods");
            }

            var columnNames = _columnNameMap[baseTableName];

            if (year < 1990)
            {
                columnNames = columnNames.Except(new[] { "sealev_dvr" }).ToList();
            }

            var result = new Dictionary<string, List<Tuple<DateTime, float>>>();

            columnNames.ForEach(cn =>
            {
                result[cn] = new List<Tuple<DateTime, float>>();
            });

            using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
            {
                conn.Open();

                var tableName = baseTableName == "sea_temp_salt"
                    ? baseTableName
                    : GenerateSeaLevTableName(year);

                try
                {
                    var query =
                        $"SELECT timeobs, {columnNames.Aggregate((current, next) => current + ", " + next)} " +
                        $"FROM {tableName} " +
                        $"WHERE statid = {stationId.AsSeaDBStationId()} " +
                        $"ORDER BY timeobs";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var timeobs = reader.GetDateTime(0);

                            columnNames
                                .Select((c, i) => new { Index = i + 1, ColumnName = c })
                                .ToList()
                                .ForEach(x =>
                                {
                                    if (!reader.IsDBNull(x.Index))
                                    {
                                        var parameterValue = reader.GetFloat(x.Index);

                                        result[x.ColumnName].Add(new Tuple<DateTime, float>(timeobs, parameterValue));
                                    }
                                });
                        }
                    }
                }
                catch (PostgresException excp)
                {
                    if (!excp.Message.Contains("does not exist"))
                    {
                        throw excp;
                    }
                }
            }

            return result;
        }

        public async Task<Dictionary<string, List<Tuple<DateTime, float>>>> RetrieveObservationsCoveredByBasisTableForStationInGivenTimeInterval(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string baseTableName,
            string stationId,
            DateTime startTime,
            DateTime endTime)
        {
            return await Task.Run(() =>
            {
                if (_columnNameMap == null)
                {
                    throw new InvalidOperationException("Please call DataProvider.Initialize before calling other methods");
                }

                endTime -= new TimeSpan(0, 0, 1);

                if (endTime.Year != startTime.Year)
                {
                    throw new InvalidOperationException("The time interval cannot span multiple years");
                }

                var columnNames = _columnNameMap[baseTableName];

                if (startTime.Year < 1990)
                {
                    columnNames = columnNames.Except(new[] { "sealev_dvr" }).ToList();
                }

                var result = new Dictionary<string, List<Tuple<DateTime, float>>>();

                columnNames.ForEach(cn =>
                {
                    result[cn] = new List<Tuple<DateTime, float>>();
                });

                using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
                {
                    conn.Open();

                    var tableName = baseTableName == "sea_temp_salt"
                        ? baseTableName
                        : GenerateSeaLevTableName(startTime.Year);

                    try
                    {
                        var query =
                            $"SELECT timeobs, {columnNames.Aggregate((current, next) => current + ", " + next)} " +
                            $"FROM {tableName} " +
                            $"WHERE statid = {stationId.AsSeaDBStationId()} " +
                            $"AND timeObs between '{startTime.AsString()}' and '{endTime.AsString()}' " +
                            $"ORDER BY timeobs";

                        using (var cmd = new NpgsqlCommand(query, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var timeobs = reader.GetDateTime(0);

                                columnNames
                                    .Select((c, i) => new { Index = i + 1, ColumnName = c })
                                    .ToList()
                                    .ForEach(x =>
                                    {
                                        if (!reader.IsDBNull(x.Index))
                                        {
                                            var parameterValue = reader.GetFloat(x.Index);

                                            result[x.ColumnName].Add(new Tuple<DateTime, float>(timeobs, parameterValue));
                                        }
                                    });
                            }
                        }
                    }
                    catch (PostgresException excp)
                    {
                        if (!excp.Message.Contains("does not exist"))
                        {
                            throw excp;
                        }
                    }
                    catch (NpgsqlException excp)
                    {
                        // Her kan vi lande, hvis forbindelsen er dårlig,
                        // som når jeg sidder i kælderen
                        throw excp;
                    }
                }

                return result;
            });
        }

        public string GetBasisTableName(
            string parameterId)
        {
            foreach (var kvp in _columnNameMap)
            {
                if (kvp.Value.Contains(parameterId))
                {
                    return kvp.Key;
                }
            }

            throw new ArgumentException("Unknown parameterId");
        }

        private List<string> GenerateBasisTableNames()
        {
            return new List<string>
            {
                "sea_temp_salt",
                "sealev"
            };
        }

        private List<string> GenerateColumnNamesForSeaLev(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "statid, timeobs, sea_reg, sealev_ln, sealev_dvr, status_ok, timeins, status_qc, status_qch";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForSeaTempSalt(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "statid, timeobs, depth_index, depth, tw, salt, status_qc, status_qch_temp, status_qch_salt";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private Dictionary<string, List<string>> GenerateColumnNameMap(
            IEnumerable<string> parametersOfInterest)
        {
            var result = new Dictionary<string, List<string>>();

            _basis_table_names.ForEach(n =>
            {
                switch (n)
                {
                    case "sealev":
                        result[n] = GenerateColumnNamesForSeaLev(parametersOfInterest);
                        break;
                    case "sea_temp_salt":
                        result[n] = GenerateColumnNamesForSeaTempSalt(parametersOfInterest);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            });

            return result;
        }

        private string GenerateSeaLevTableName(
            int startYear)
        {
            if (startYear < 1878)
            {
                throw new ArgumentException();
            }

            if (startYear < 1970)
            {
                return $"sealev_1878_69";
            }

            if (startYear < 1980)
            {
                return $"sealev_1970_79";
            }

            if (startYear < 1990)
            {
                return $"sealev_1980_89";
            }

            if (startYear < 2000)
            {
                return $"sealev_1990_99";
            }

            if (startYear < 2010)
            {
                return $"sealev_2000_09";
            }

            if (startYear < 2020)
            {
                return $"sealev_2010_19";
            }

            if (startYear < 2030)
            {
                return $"sealev_2020_29";
            }

            throw new ArgumentException();
        }

        private List<string> GenerateSeaLevColumnNamesForQuery(
            string baseTableName,
            int startYear)
        {
            if (startYear > 1989)
            {
                return _columnNameMap[baseTableName];
            }

            return _columnNameMap[baseTableName].Except(new string[] { "sealev_dvr" }).ToList();
        }
    }
}
