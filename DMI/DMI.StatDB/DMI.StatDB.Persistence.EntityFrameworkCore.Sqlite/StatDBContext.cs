using DMI.StatDB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class StatDBContext : DbContext
    {
        public DbSet<Station> StationInformations { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }


    }
}
