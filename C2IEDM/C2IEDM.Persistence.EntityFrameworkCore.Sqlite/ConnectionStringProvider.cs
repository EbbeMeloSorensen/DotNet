namespace C2IEDM.Persistence.EntityFrameworkCore.Sqlite;

public static class ConnectionStringProvider
{
    public static string GetConnectionString()
    {
        return "Data source=c2iedm.db";
    }
}