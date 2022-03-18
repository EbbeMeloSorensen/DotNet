using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.UnitTest;

public class TestHelper
{
    public static DbProviderFactory DbProviderFactory { get; }

    public static string ConnectionString { get; }

    public static void ClearTable(
        string tableName)
    {
        ExecuteNonQuery($"DELETE FROM {tableName}");
    }

    public static void InsertRowInTable(
        string tableName, 
        string columnsAndValues, 
        bool withIdentityInsert = true)
    {
        var commandText =
            $"INSERT {tableName} " +
            $"{columnsAndValues};";

        if (withIdentityInsert)
        {
            commandText =
                $"SET IDENTITY_INSERT {tableName} ON;" +
                commandText +
                $"SET IDENTITY_INSERT {tableName} OFF";
        }

        ExecuteNonQuery(commandText);
    }

    static TestHelper()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
        DbProviderFactory = DbProviderFactories.GetFactory("Microsoft.Data.SqlClient");

        ConnectionStringProvider.Initialize("melo-home\\sqlexpress", "StatDB_Test", "sa", "L1on8Zebra");
        ConnectionString = ConnectionStringProvider.GetConnectionString();
    }

    private static void ExecuteNonQuery(string commandText)
    {
        using var dbConnection = DbProviderFactory.CreateConnection();
        dbConnection.ConnectionString = ConnectionString;
        dbConnection.Open();

        using var dbCommand = DbProviderFactory.CreateCommand();
        dbCommand.Connection = dbConnection;
        dbCommand.CommandText = commandText;
        dbCommand.ExecuteNonQuery();
    }
}