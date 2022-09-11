namespace PR.Persistence.EntityFrameworkCore.Sqlite
{
    public static class ConnectionStringProvider
    {
        private static string _connectionString;

        public static void Initialize()
        {
            _connectionString = "Data source=people.db";
        }

        public static string GetConnectionString()
        {
            return "Data source=people.db";
        }
    }
}