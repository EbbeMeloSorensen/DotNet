using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer
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
                host = "melo-home\\sqlexpress";
                database = "Glossary";
                user = "sa";
                password = "L1on8Zebra";
            }

            Initialize(host, database, user, password);
        }

        public static void Initialize(
            string host,
            string database,
            string user,
            string password)
        {
            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(password))
            {
                return;
            }

            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = host,
                InitialCatalog = database,
                UserID = user,
                Password = password
            };
            
            _connectionString = sqlConnectionStringBuilder.ToString();
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
