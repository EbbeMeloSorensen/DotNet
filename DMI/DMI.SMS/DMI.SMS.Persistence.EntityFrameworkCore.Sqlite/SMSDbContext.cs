using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite
{
    public class SMSDbContext : DbContext
    {
        public DbSet<StationInformation> StationInformations { get; set; }
        public DbSet<SensorLocation> SensorLocations { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StationInformationConfiguration());
            modelBuilder.ApplyConfiguration(new SensorLocationConfiguration());
        }
    }
}