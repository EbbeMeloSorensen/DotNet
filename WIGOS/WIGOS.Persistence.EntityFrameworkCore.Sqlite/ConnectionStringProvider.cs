namespace WIGOS.Persistence.EntityFrameworkCore.Sqlite
{
    public static class ConnectionStringProvider
    {
        public static string GetConnectionString()
        {
            return "Data source=wigos.db";
        }
    }
}