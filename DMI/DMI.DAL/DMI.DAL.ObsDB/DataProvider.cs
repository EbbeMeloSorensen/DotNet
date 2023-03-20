using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Craft.Logging;
using DMI.Utils;

namespace DMI.DAL.ObsDB
{
    public class DataProvider : DataProviderBase
    {
        private readonly Dictionary<string, int> _basis_table_names;
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
            return await Task.Run(async () =>
            {
                var aggregatedResult = new Dictionary<string, int>();

                //_basis_table_names.Keys.ToList().ForEach(async btn =>
                //{
                //    var result = await CountObservationsCoveredByBasisTableForStation(
                //        host, database, obsDBUser, obsDBPassword,
                //        btn, stationId, startTime, endTime, includeZeroCountParams);

                //    aggregatedResult.Aggregate(result);
                //});

                foreach (var btn in _basis_table_names.Keys)
                {
                    var result = await CountObservationsCoveredByBasisTableForStation(
                        host, database, obsDBUser, obsDBPassword,
                        btn, stationId, startTime, endTime, includeZeroCountParams);

                    aggregatedResult.Aggregate(result);

                    await Task.Delay(300);
                }

                return aggregatedResult;
            });
        }

        public async Task<Dictionary<string, int>> CountObservationsCoveredByBasisTableForStation(
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

            var message = $"      Counting observations from table {baseTableName} observations for {stationId}..";
            _logger?.WriteLine(LogMessageCategory.Information, message);

            await Task.Delay(100);

            return await Task.Run(async () =>
            {
                var result = new Dictionary<string, int>();

                var parameterIds = _columnNameMap[baseTableName];

                if (parameterIds.Count == 0)
                {
                    return result;
                }

                if (includeZeroCountParams)
                {
                    parameterIds.ForEach(paramId =>
                    {
                        result[paramId] = 0;
                    });
                }

                var numberOfAttemptsBeforeAborting = 10;
                var retryDelayInSeconds = 5;

                var attempt = 1;

                while (attempt <= numberOfAttemptsBeforeAborting)
                {
                    if (attempt > 1)
                    {
                        message = $"Attempt {attempt} of counting observations from table {baseTableName} observations for {stationId}..";
                        _logger?.WriteLine(LogMessageCategory.Information, message);
                        await Task.Delay(100);
                    }

                    try
                    {
                        using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
                        {
                            conn.Open();

                            var query = string.Concat(
                                "SELECT ",
                                parameterIds
                                    .Select(columnName => $"sum(case when {columnName} is null then 0 else 1 end) as {columnName}")
                                    .Aggregate((c, n) => $"{c}, {n}"),
                                $" FROM {baseTableName}_{startTime.Year} ",
                                $"WHERE best = true ",
                                $"AND statid = {stationId.AsNanoqStationId()} ",
                                $"AND timeObs between '{startTime.AsDateTimeString(false)}' and '{endTime.AsDateTimeString(false)}'");

                            using (var cmd = new NpgsqlCommand(query, conn))
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    _columnNameMap[baseTableName]
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

                                reader.Close();
                            }

                            conn.Close();
                        }

                        break; // break out of the while loop because the API call succeeded
                    }
                    catch (PostgresException excp)
                    {
                        if (excp.Message.Contains("does not exist"))
                        {
                            // Det er normalt den lander her - det er nemlig ikke altid at tabellen er der, hvis den f.eks. ike går helt tilabge til 1953
                            break; // break out of the while loop because the API call succeeded
                        }
                        else
                        {
                            _logger?.WriteLine(LogMessageCategory.Information, "counting attempt failed (PostgresException)");

                            await Task.Delay(100);

                            throw excp;
                        }
                    }
                    catch (NpgsqlException excp)
                    {
                        _logger?.WriteLine(LogMessageCategory.Information, $"counting attempt failed (NpgsqlException, message: {excp.Message})");

                        await Task.Delay(100);
                    }

                    Thread.Sleep(retryDelayInSeconds * 1000);
                    attempt++;
                }

                if (attempt == numberOfAttemptsBeforeAborting)
                {
                    var errorMessage = $"Fatal Error: counting attempt failed {numberOfAttemptsBeforeAborting} times. Aborting";
                    _logger?.WriteLine(LogMessageCategory.Information, errorMessage);
                    throw new Exception(errorMessage);
                }

                return result;
            });
        }

        public int CountObservationsOfIndividualParameterForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string paramId,
            string stationId,
            DateTime startTime,
            DateTime endTime)
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

            var result = 0;

            using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
            {
                conn.Open();

                try
                {
                    var query =
                        $"SELECT COUNT({paramId}) " +
                        $"FROM {GetBasisTableName(paramId)}_{startTime.Year} " +
                        $"WHERE best = true " +
                        $"AND statid = {stationId.AsNanoqStationId()} " +
                        $"AND timeObs between '{startTime.AsDateTimeString(false)}' and '{endTime.AsDateTimeString(false)}'";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            result = count;
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

        public Dictionary<string, List<Tuple<DateTime, float>>> RetrieveObservationsForStationInGivenYear(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId,
            int year)
        {
            if (_columnNameMap == null)
            {
                throw new InvalidOperationException("Please call DataProvider.Initialize before calling other methods");
            }

            var result = new Dictionary<string, List<Tuple<DateTime, float>>>();

            _basis_table_names.Keys.ToList().ForEach(btn =>
            {
                var temp = RetrieveObservationsCoveredByBasisTableForStationInGivenYear(
                    host, database, obsDBUser, obsDBPassword, btn, stationId, year);
            });

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
            var result = new Dictionary<string, List<Tuple<DateTime, float>>>();

            columnNames.ForEach(cn =>
            {
                result[cn] = new List<Tuple<DateTime, float>>();
            });

            using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
            {
                conn.Open();

                try
                {
                    var query =
                        $"SELECT timeobs, {columnNames.Aggregate((current, next) => current + ", " + next)} " +
                        $"FROM {baseTableName}_{year} " +
                        $"WHERE best = true " +
                        $"AND statid = {stationId.AsNanoqStationId()} " +
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
                var result = new Dictionary<string, List<Tuple<DateTime, float>>>();

                columnNames.ForEach(cn =>
                {
                    result[cn] = new List<Tuple<DateTime, float>>();
                });

                using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
                {
                    conn.Open();

                    try
                    {
                        var query =
                            $"SELECT timeobs, {columnNames.Aggregate((current, next) => current + ", " + next)} " +
                            $"FROM {baseTableName}_{startTime.Year} " +
                            $"WHERE best = true " +
                            $"AND statid = {stationId.AsNanoqStationId()} " +
                            $"AND timeObs between '{startTime.AsDateTimeString(false)}' and '{endTime.AsDateTimeString(false)}' " +
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
            });
        }

        public async Task<DateTime?> IdentifyTimeForOldestObservationForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId)
        {
            return await Task.Run(() =>
            {
                var latestYear = DateTime.Now.Year - 1;
                var year = _basis_table_names.Values.Min();

                var result = new DateTime?();

                do
                {
                    _logger?.WriteLine(LogMessageCategory.Information, $"  Inspecting year {year}");

                    _basis_table_names.Keys.ToList().ForEach(btn =>
                    {
                        var time = IdentifyExtremumTimeForObservationCoveredBySpecificTableForStation(
                            host, database, obsDBUser, obsDBPassword, btn, year, stationId, false);

                        if (!result.HasValue ||
                            time.HasValue && time.Value < result.Value)
                        {
                            result = time;
                        }
                    });

                    if (result.HasValue)
                    {
                        return result;
                    }

                    year++;
                } while (year <= latestYear);

                return result;
            });
        }

        public async Task<DateTime?> IdentifyTimeForMostRecentObservationForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string stationId)
        {
            return await Task.Run(() =>
            {
                var earliestYear = _basis_table_names.Values.Min();
                var year = DateTime.Now.Year - 1;

                var result = new DateTime?();

                do
                {
                    _logger?.WriteLine(LogMessageCategory.Information, $"  Inspecting year {year}");

                    _basis_table_names.Keys.ToList().ForEach(btn =>
                    {
                        var time = IdentifyExtremumTimeForObservationCoveredBySpecificTableForStation(
                            host, database, obsDBUser, obsDBPassword, btn, year, stationId, true);

                        if (!result.HasValue ||
                            time.HasValue && time.Value < result.Value)
                        {
                            result = time;
                        }
                    });

                    if (result.HasValue)
                    {
                        return result;
                    }

                    year--;
                } while (year >= earliestYear);

                return result;
            });
        }

        public async Task<DateTime?> IdentifyTimeForOldestObservationCoveredByBasisTableForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string basisTableName,
            string stationId)
        {
            return await Task.Run(() =>
            {
                DateTime? oldestDateTime = null;

                var latestYear = DateTime.Now.Year - 1;
                var year = _basis_table_names[basisTableName];

                do
                {
                    var timeOfOldestObservationForTable = IdentifyExtremumTimeForObservationCoveredBySpecificTableForStation(
                        host, database, obsDBUser, obsDBPassword, basisTableName, year, stationId, false);

                    if (timeOfOldestObservationForTable.HasValue)
                    {
                        if (!oldestDateTime.HasValue ||
                            timeOfOldestObservationForTable.Value < oldestDateTime.Value)
                        {
                            oldestDateTime = timeOfOldestObservationForTable;
                            return oldestDateTime;
                        }
                    }

                    year++;
                } while (year <= DateTime.Now.Year - 1);

                return oldestDateTime;
            });
        }

        public async Task<DateTime?> IdentifyTimeForMostRecentObservationCoveredByBasisTableForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string basisTableName,
            string stationId)
        {
            return await Task.Run(() =>
            {
                DateTime? mostRecentDateTime = null;

                var earliestYear = _basis_table_names[basisTableName];
                var year = DateTime.Now.Year - 1;

                do
                {
                    var timeOfMostRecentObservationForTable = IdentifyExtremumTimeForObservationCoveredBySpecificTableForStation(
                        host, database, obsDBUser, obsDBPassword, basisTableName, year, stationId, true);

                    if (timeOfMostRecentObservationForTable.HasValue)
                    {
                        if (!mostRecentDateTime.HasValue ||
                            timeOfMostRecentObservationForTable.Value < mostRecentDateTime.Value)
                        {
                            mostRecentDateTime = timeOfMostRecentObservationForTable;
                            return mostRecentDateTime;
                        }
                    }

                    year--;
                } while (year >= earliestYear);

                return mostRecentDateTime;
            });
        }

        public DateTime? IdentifyExtremumTimeForObservationCoveredBySpecificTableForStation(
            string host,
            string database,
            string obsDBUser,
            string obsDBPassword,
            string basisTableName,
            int year,
            string stationId,
            bool maximum)
        {
            DateTime? result = null;

            if (_basis_table_names[basisTableName] > year)
            {
                return null;
            }

            using (var conn = new NpgsqlConnection(ConnectionString(host, database, obsDBUser, obsDBPassword)))
            {
                conn.Open();

                try
                {
                    // Denne query kan åbenbart være temmelig tung.....
                    // Eller skyldes det, at nanoq er nede at bide i øjeblikket?

                    var query = string.Concat(
                        "SELECT timeObs",
                        $" FROM {basisTableName}_{year}",
                        $" WHERE statid = {stationId.AsNanoqStationId()}",
                        //" AND status_ok = 1", // Nogle af tabellerne har ikke denne kolonne
                        " ORDER BY timeObs",
                        maximum ? " DESC" : " ASC",
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
            }

            return result;
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

        private Dictionary<string, int> GenerateBasisTableNames()
        {
            return new Dictionary<string, int>
            {
                { "precip_hum_pressure", 1953 },
                { "precip_hum_pressure_1", 1987 },
                { "precip_past1min", 2012 },
                { "snow_man", 1971 },
                { "temp_wind_radiation", 1953 },
                { "temp_wind_radiation_1", 1961 },
                { "visibility_weather_cloud_ground", 1953 },
                { "visibility_weather_cloud_ground_1", 1999 }
            };
        }

        private List<string> GenerateColumnNamesForPrecipHumPressure(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, pressure, pressure_qc1, pressure_at_sea," +
                "pressure_at_sea_qc1, pressure_change_last3h, pressure_change_last3h_qc1," +
                "pressure_character, pressure_character_qc1, humidity, humidity_qc1," +
                "snow_depth, snow_depth_qc1, precip_past3h, precip_past3h_qc1," +
                "precip_past6h, precip_past6h_qc1, precip_past12h, precip_past12h_qc1," +
                "precip_past24h, precip_past24h_qc1, humidity_past1h, humidity_past1h_qc1," +
                "precip_past10min, precip_past10min_qc1, precip_max_last10min," +
                "precip_max_last10min_qc1, precip_past1h, precip_past1h_qc1, precip_max_past1h," +
                "precip_max_past1h_qc1, precip_height, precip_past24h_height";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForPrecipHumPressure_1(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, precip_max_per15s_last10min_optic," +
                "precip_max_per15s_last10min_optic_qc1, precip_mean_per15s_last10min_optic," +
                "precip_mean_per15s_last10min_optic_qc1, precip_per3h_optic, precip_per3h_optic_qc1," +
                "snow_per3h_optic, snow_per3h_optic_qc1, precip_per10min_optic," +
                "precip_per10min_optic_qc1, precip_dur_per3h, precip_dur_per3h_qc1," +
                "precip_dur_per10min, precip_dur_per10min_qc1, precip_dur_past1h," +
                "precip_dur_past1h_qc1, leav_hum_dur_past10min, leav_hum_dur_past10min_qc1," +
                "precip_dur_past10min, precip_dur_past10min_qc1, leav_hum_dur_past1h," +
                "leav_hum_dur_past1h_qc1, precip_max_per15s_last6h_optic, precip_max_per15s_last6h_optic_qc1," +
                "precip_max_per15s_last3h_optic, precip_max_per15s_last3h_optic_qc1," +
                "precip_max_per15s_last1h_optic, precip_max_per15s_last1h_optic_qc1";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForPrecipPast1Min(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, precip_past1min";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForSnowMan(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, snow_depth_man, snow_depth_man_qc1," +
                "snow_cover_man, snow_cover_man_qc1";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForTempWindRadiation(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, wind_dir, wind_dir_qc1, wind_speed," +
                "wind_speed_qc1, wind_gust_past10min, wind_gust_past10min_qc1," +
                "wind_dir_past1h, wind_dir_past1h_qc1, wind_speed_past1h, wind_speed_past1h_qc1," +
                "wind_max, wind_max_qc1, wind_gust_always_past1h, wind_gust_always_past1h_qc1," +
                "temp_dry, temp_dry_qc1, temp_dew, temp_dew_qc1, temp_max_past12h," +
                "temp_max_past12h_qc1, temp_min_past12h, temp_min_past12h_qc1," +
                "temp_mean_past1h, temp_mean_past1h_qc1, temp_max_past1h, temp_max_past1h_qc1," +
                "temp_min_past1h, temp_min_past1h_qc1, radia_glob, radia_glob_qc1," +
                "sun_last10min_glob, sun_last10min_glob_qc1, radia_glob_past1h," +
                "radia_glob_past1h_qc1, sun_last1h_glob, sun_last1h_glob_qc1," +
                "wind_gust_last6h, wind_gust_last6h_qc1, temp_grass, temp_grass_qc1," +
                "wind_gust_last3h, wind_gust_last3h_qc1, wind_gust_last1h, wind_gust_last1h_qc1," +
                "wind_max_per10min_past1h, wind_max_per10min_past1h_qc1, temp_extr_height," +
                "wind_height, wind_instrumentation";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForTempWindRadiation_1(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, wind_dir_std_dev, wind_dir_std_dev_qc1," +
                "wind_min, wind_min_qc1, wind_dir_std_dev_past1h, wind_dir_std_dev_past1h_qc1," +
                "wind_min_past1h, wind_min_past1h_qc1, wind_min_per10min_past1h," +
                "wind_min_per10min_past1h_qc1, temp_grass_min_past12h, temp_grass_min_past12h_qc1," +
                "radia_uvb, radia_uvb_qc1, sun_last10min_sun, sun_last10min_sun_qc1," +
                "radia_uvb_past1h, radia_uvb_past1h_qc1, sun_last1h_sun, sun_last1h_sun_qc1," +
                "wind_max_per10min_past6h, wind_max_per10min_past6h_qc1, temp_grass_mean_past1h," +
                "temp_grass_mean_past1h_qc1, temp_grass_max_past1h, temp_grass_max_past1h_qc1," +
                "temp_grass_min_past1h, temp_grass_min_past1h_qc1, wind_max_per10min_past3h," +
                "wind_max_per10min_past3h_qc1, temp_soil, temp_soil_qc1, temp_soil_mean_past1h," +
                "temp_soil_mean_past1h_qc1, temp_soil_max_past1h, temp_soil_max_past1h_qc1," +
                "temp_soil_min_past1h, temp_soil_min_past1h_qc1, temp_soil_at30cm," +
                "temp_soil_at30cm_qc1";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForVisibilityWeatherCloudGround(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, visibility, visibility_qc1, weather," +
                "weather_qc1, cloud_cover, cloud_cover_qc1, cloud_cover_low_med," +
                "cloud_cover_low_med_qc1, cloud_height, cloud_height_qc1, weather1_past6h," +
                "weather1_past6h_qc1, weather2_past6h, weather2_past6h_qc1, cloud_type_low," +
                "cloud_type_low_qc1, weather1_past3h, weather1_past3h_qc1, weather2_past3h," +
                "weather2_past3h_qc1, cloud_type_med, cloud_type_med_qc1, weather1_past1h," +
                "weather1_past1h_qc1, weather2_past1h, weather2_past1h_qc1, cloud_type_high," +
                "cloud_type_high_qc1, ground_state, ground_state_qc1, cloud_type_code," +
                "cloud_type_code_qc1, visibility_height";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        private List<string> GenerateColumnNamesForVisibilityWeatherCloudGround_1(
            IEnumerable<string> parametersOfInterest)
        {
            var stringGeneratedByPGAdmin =
                "insid, statid, timeobs, best, visib_min_per1min_last10min, visib_min_per1min_last10min_qc1," +
                "visib_max_per1min_last10min, visib_max_per1min_last10min_qc1," +
                "visib_min_per1min_last6h, visib_min_per1min_last6h_qc1, visib_max_per1min_last6h," +
                "visib_max_per1min_last6h_qc1, visib_min_per1min_last3h, visib_min_per1min_last3h_qc1," +
                "visib_max_per1min_last3h, visib_max_per1min_last3h_qc1, visib_min_per1min_last1h," +
                "visib_min_per1min_last1h_qc1, visib_max_per1min_last1h, visib_max_per1min_last1h_qc1," +
                "visib_mean_last10min, visib_mean_last10min_qc1";

            return stringGeneratedByPGAdmin
                .Split(',')
                .Select(s => s.Trim())
                .Intersect(parametersOfInterest)
                .ToList();
        }

        // Generates a map where you can look up the parameters that are covered by the various basis table names
        private Dictionary<string, List<string>> GenerateColumnNameMap(
            IEnumerable<string> parametersOfInterest)
        {
            var result = new Dictionary<string, List<string>>();

            _basis_table_names.Keys.ToList().ForEach(n =>
            {
                switch (n)
                {
                    case "precip_hum_pressure":
                        result[n] = GenerateColumnNamesForPrecipHumPressure(parametersOfInterest);
                        break;
                    case "precip_hum_pressure_1":
                        result[n] = GenerateColumnNamesForPrecipHumPressure_1(parametersOfInterest);
                        break;
                    case "precip_past1min":
                        result[n] = GenerateColumnNamesForPrecipPast1Min(parametersOfInterest);
                        break;
                    case "snow_man":
                        result[n] = GenerateColumnNamesForSnowMan(parametersOfInterest);
                        break;
                    case "temp_wind_radiation":
                        result[n] = GenerateColumnNamesForTempWindRadiation(parametersOfInterest);
                        break;
                    case "temp_wind_radiation_1":
                        result[n] = GenerateColumnNamesForTempWindRadiation_1(parametersOfInterest);
                        break;
                    case "visibility_weather_cloud_ground":
                        result[n] = GenerateColumnNamesForVisibilityWeatherCloudGround(parametersOfInterest);
                        break;
                    case "visibility_weather_cloud_ground_1":
                        result[n] = GenerateColumnNamesForVisibilityWeatherCloudGround_1(parametersOfInterest);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            });

            return result;
        }
    }
}
