namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite
{
    public static class ConnectionStringProvider
    {
        private static string _connectionString;

        static ConnectionStringProvider()
        {
            _connectionString = "Data source=sms.db";
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
