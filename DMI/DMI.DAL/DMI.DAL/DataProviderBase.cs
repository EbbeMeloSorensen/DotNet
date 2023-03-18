using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using Npgsql;
using Craft.Logging;

namespace DMI.DAL
{
    public class DataProviderBase
    {
        protected ILogger _logger;

        public DataProviderBase(
            ILogger logger)
        {
            _logger = logger;
        }

        protected static string ConnectionString(
            string host,
            string database,
            string user,
            string password)
        {
            if (host == null ||
                database == null ||
                user == null ||
                password == null)
            {
                throw new InvalidOperationException("Connection string invalid due to missing parameters");
            }

            return $"Host={host};Username={user};Password={password};Database={database}";
        }

        public async Task<bool> CheckConnection(
            string host,
            string database,
            string user,
            string password)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger?.WriteLine(LogMessageCategory.Information, $"Checking connection to database: {database} on host: {host}");

                    using (var conn = new NpgsqlConnection(ConnectionString(host, database, user, password)))
                    {
                        conn.Open();
                        _logger?.WriteLine(LogMessageCategory.Information, "Connection is OK");
                        return true;
                    }
                }
                catch (SocketException e)
                {
                    _logger?.WriteLine(LogMessageCategory.Information, $"Socket Exception: \"{e.Message}\"");
                    return false;
                }
                catch (PostgresException e)
                {
                    _logger?.WriteLine(LogMessageCategory.Information, $"PostgresException Exception: \"{e.Message}\"");
                    return false;
                }
                catch (NpgsqlException e)
                {
                    _logger?.WriteLine(LogMessageCategory.Information, $"NpgsqlException Exception: \"{e.Message}\"");
                    return false;
                }
            });
        }
    }
}
