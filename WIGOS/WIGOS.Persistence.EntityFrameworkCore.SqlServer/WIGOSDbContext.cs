using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.SqlServer
{
    public class WIGOSDbContext : WIGOSDbContextBase
    {
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}