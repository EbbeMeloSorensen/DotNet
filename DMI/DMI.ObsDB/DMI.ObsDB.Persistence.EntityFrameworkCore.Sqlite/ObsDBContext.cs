using Microsoft.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite
{
    public class ObsDBContext : DbContext
    {
        public DbSet<ObservingFacility> ObservingFacilities { get; set; }
        public DbSet<TimeSeries> TimeSeries { get; set; }
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
            modelBuilder.ApplyConfiguration(new ObservingFacilityConfiguration());
            modelBuilder.ApplyConfiguration(new TimeSeriesConfiguration());
            modelBuilder.ApplyConfiguration(new ObservationConfiguration());

            modelBuilder.Entity<TimeSeries>()
                .HasOne(_ => _.ObservingFacility)
                .WithMany(_ => _.TimeSeries)
                .HasForeignKey(_ => _.ObservingFacilityId);
        }
    }
}
