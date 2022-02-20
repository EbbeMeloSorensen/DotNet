using System.Configuration;

namespace DMI.StatDB.Persistence.Npgsql
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
            var host = settings["StatDB_PostgreSQL_Host"]?.Value;
            var port = settings["StatDB_PostgreSQL_Port"]?.Value;
            var database = settings["StatDB_PostgreSQL_Database"]?.Value;
            var schema = settings["StatDB_PostgreSQL_Schema"]?.Value;
            var userID = settings["StatDB_PostgreSQL_UserID"]?.Value;
            var password = settings["StatDB_PostgreSQL_Password"]?.Value;

            Initialize(host, string.IsNullOrEmpty(port) ? 5432 : int.Parse(port), database, schema, userID, password);
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
