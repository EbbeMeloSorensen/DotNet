using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore
{
    public class SMSDbContextBase : DbContext
    {
        public DbSet<StationInformation> StationInformations { get; set; }
        public DbSet<SensorLocation> SensorLocations { get; set; }
        public DbSet<ElevationAngles> ElevationAngles { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StationInformationConfiguration());
            modelBuilder.ApplyConfiguration(new SensorLocationConfiguration());
            modelBuilder.ApplyConfiguration(new ElevationAnglesConfiguration());

            modelBuilder.Entity<ElevationAngles>()
                .HasOne(_ => _.SensorLocation)
                .WithMany()
                .HasForeignKey(_ => _.ParentGdbArchiveOid)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}