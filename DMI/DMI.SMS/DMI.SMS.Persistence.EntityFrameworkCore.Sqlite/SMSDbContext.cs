using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite
{
    public class SMSDbContext : DbContext
    {
        public DbSet<StationInformation> StationInformations { get; set; }
        public DbSet<SensorLocation> SensorLocations { get; set; }
        public DbSet<ElevationAngles> ElevationAngles { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            builder.ApplyConfiguration(new StationInformationConfiguration());
            builder.ApplyConfiguration(new SensorLocationConfiguration());
            builder.ApplyConfiguration(new ElevationAnglesConfiguration());

            builder.Entity<ElevationAngles>()
                .HasOne(_ => _.SensorLocation)
                .WithMany()
                .HasForeignKey(_ => _.ParentGdbArchiveOid)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}