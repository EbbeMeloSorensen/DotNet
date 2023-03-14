using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using Npgsql;

namespace DMI.DAL.StatDB
{
    public class DataProvider : DataProviderBase
    {
        public DataProvider(ILogger logger) : base(logger)
        {
        }

        public async Task<List<int>> RetrieveDataFromStationInformationTable(
            string host,
            string database,
            string statDBUser,
            string statDBPassword)
        {
            return await Task.Run(() =>
            {
                var result = new List<int>();

                using (var conn = new NpgsqlConnection(ConnectionString(host, database, statDBUser, statDBPassword)))
                {
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

                                    result.Add(statid);
                                }
                            }
                        }
                    }
                    catch (PostgresException excp)
                    {
                        throw excp;
                    }
                }

                return result;
            });
        }

        public async Task<Dictionary<int, List<string>>> RetrieveStationParameterMapFromStatParameterTable(
            string host,
            string database,
            string statDBUser,
            string statDBPassword)
        {
            return await Task.Run(() =>
            {
                var result = new Dictionary<int, List<string>>();

                using (var conn = new NpgsqlConnection(ConnectionString(host, database, statDBUser, statDBPassword)))
                {
                    conn.Open();

                    try
                    {
                        var query = "SELECT statid, parameter FROM stat_parameter";

                        var parameters = new List<string>();

                        using (var cmd = new NpgsqlCommand(query, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var statid = reader.GetInt32(0);
                                var parameter = reader.GetString(1);

                                if (!result.ContainsKey(statid))
                                {
                                    result[statid] = new List<string>();
                                }

                                if (!result[statid].Contains(parameter))
                                {
                                    result[statid].Add(parameter);
                                }
                            }
                        }
                    }
                    catch (PostgresException excp)
                    {
                        throw excp;
                    }
                }

                result = result
                .Select(kvp => new KeyValuePair<int, List<string>>(kvp.Key, kvp.Value.OrderBy(p => p).ToList()))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                return result;
            });
        }
    }
}
