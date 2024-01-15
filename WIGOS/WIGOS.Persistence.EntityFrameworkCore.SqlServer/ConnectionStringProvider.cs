using Microsoft.Data.SqlClient;
using System.Configuration;

namespace WIGOS.Persistence.EntityFrameworkCore.SqlServer
{
    public static class ConnectionStringProvider
    {
        private static string _connectionString;

        static ConnectionStringProvider()
        {
            InitializeFromSettingsFile();
        }

        public static void InitializeFromSettingsFile()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var host = settings["Host"]?.Value;
            var database = settings["Database"]?.Value;
            var user = settings["User"]?.Value;
            var password = settings["Password"]?.Value;

            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(password))
            {
                // If we are here, it may be because we're attempting to generate a migration
                host = "localhost";
                database = "PR";
                user = "sa";
                password = "L1on8Zebra";
            }

            //Initialize(host, string.IsNullOrEmpty(port) ? 5432 : int.Parse(port), database, schema, user, password);
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

                var defaultConnStringBuilder = new SqlConnectionStringBuilder
                {
                    UserID = "sa",
                    Password = "L1on8Zebra",
                    InitialCatalog = "WIGOS_II",
                    DataSource = "melo-home\\sqlexpress"
                };

                return defaultConnStringBuilder.ToString();
            }

            return _connectionString;
        }
    }
}