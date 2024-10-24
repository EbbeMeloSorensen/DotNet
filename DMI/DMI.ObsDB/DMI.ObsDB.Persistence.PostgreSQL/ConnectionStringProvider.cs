﻿using System.Configuration;

namespace DMI.ObsDB.Persistence.PostgreSQL
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
            var host = settings["Host_Source"]?.Value;
            var port = settings["Port_Source"]?.Value;
            var database = settings["Database_Source"]?.Value;
            var schema = settings["Schema_Source"]?.Value;
            var user = settings["User_Source"]?.Value;
            var password = settings["Password_Source"]?.Value;

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
