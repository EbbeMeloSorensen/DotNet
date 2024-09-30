using System.Configuration;

namespace DMI.SMS.Persistence.EntityFrameworkCore.PostgreSQL
{
    public static class ConnectionStringProvider
    {
        private static string _schema;
        private static string _connectionString;

        static ConnectionStringProvider()
        {
            InitializeFromSettingsFile();
        }

        public static void InitializeFromSettingsFile()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var host = settings["SMS_PostgreSQL_Host"]?.Value;
            var port = settings["SMS_PostgreSQL_Port"]?.Value;
            var database = settings["SMS_PostgreSQL_Database"]?.Value;
            var schema = settings["SMS_PostgreSQL_Schema"]?.Value;
            var user = settings["SMS_PostgreSQL_UserID"]?.Value;
            var password = settings["SMS_PostgreSQL_Password"]?.Value;

            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(port) ||
                string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(schema) ||
                string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(password))
            {
                // If we are here, it may be because we're attempting to generate a migration
                host = "localhost";
                port = "5432";
                database = "PR";
                schema = "public";
                user = "postgres";
                password = "L1on8Zebra";
            }

            Initialize(host, string.IsNullOrEmpty(port) ? 5432 : int.Parse(port), database, schema, user, password);
        }

        public static void Initialize(
            string host,
            int port,
            string database,
            string schema,
            string user,
            string password)
        {
            _schema = schema;

            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(schema) ||
                string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(password))
            {
                return;
            }

            _connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }

        public static string GetPostgreSqlSchema()
        {
            return _schema;
        }
    }
}
