using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.SqlServer;

public class C2IEDMDbContext : C2IEDMDbContextBase
{
    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = ConnectionStringProvider.GetConnectionString();
        optionsBuilder.UseSqlServer(connectionString);
    }
}