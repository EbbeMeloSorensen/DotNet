namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite
{
    public static class ConnectionStringProvider
    {
        private static string _connectionString;

        static ConnectionStringProvider()
        {
            _connectionString = "Data source=obsdb.db";
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }
    }
}