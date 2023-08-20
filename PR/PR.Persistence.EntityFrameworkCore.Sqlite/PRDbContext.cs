using Microsoft.EntityFrameworkCore;

namespace PR.Persistence.EntityFrameworkCore.Sqlite
{
    public class PRDbContext : PRDbContextBase
    {
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}
