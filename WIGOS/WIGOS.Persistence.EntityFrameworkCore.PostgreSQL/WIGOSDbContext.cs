using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class WIGOSDbContext : WIGOSDbContextBase
    {
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}