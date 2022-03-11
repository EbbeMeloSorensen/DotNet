using System.Configuration;
using System.Data.SqlClient;

namespace DMI.StatDB.Persistence.EntityFramework
{
    public static class ConnectionStringProvider
    {
        private static string _connectionString;

        static ConnectionStringProvider()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var host = settings["Host"]?.Value;
            var initialCatalog = settings["InitialCatalog"]?.Value;
            var userID = settings["UserID"]?.Value;
            var password = settings["Password"]?.Value;

            if (host != null &&
                initialCatalog != null &&
                userID != null &&
                password != null)
            {
                Initialize(host, initialCatalog, userID, password);
            }
        }

        public static void Initialize(
            string host,
            string initialCatalog,
            string userID,
            string password)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = host,
                InitialCatalog = initialCatalog,
                UserID = userID,
                Password = password
            };

            _connectionString = sqlConnectionStringBuilder.ToString();
        }

        public static string GetConnectionString()
        {
            if (_connectionString == null)
            {
                // If we are here, it may be because we are enabling migrations with the Package Manager Console,
                // Then, we should just return a connection string for the repository on MELO-HOME, where we are
                // developing the solution.
                // Todo: read it from a file instead - it shouldn't be part of source code

                var defaultConnStringBuilder = new SqlConnectionStringBuilder
                {
                    UserID = "sa",
                    Password = "L1on8Zebra",
                    InitialCatalog = "SMS",
                    DataSource = "melo-home\\sqlexpress"
                };

                return defaultConnStringBuilder.ToString();
            }

            return _connectionString;
        }
    }
}
