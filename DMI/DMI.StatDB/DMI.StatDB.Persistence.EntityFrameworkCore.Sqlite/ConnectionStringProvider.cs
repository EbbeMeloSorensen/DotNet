namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite
{
    public static class ConnectionStringProvider
    {
        private static string _connectionString;

        static ConnectionStringProvider()
        {
            _connectionString = "Data source=statdb.db";
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }
    }
}