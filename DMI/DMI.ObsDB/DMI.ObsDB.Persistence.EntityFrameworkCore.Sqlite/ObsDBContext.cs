using Microsoft.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class ObsDBContext : DbContext
    {
        public DbSet<Observation> Observations { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ObservationConfiguration());
        }
    }
}
