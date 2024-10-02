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
        public DbSet<ServiceVisitReport> ServiceVisitReports { get; set; }
        public DbSet<ContactPerson> ContactPersons { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StationInformationConfiguration());
            modelBuilder.ApplyConfiguration(new ContactPersonConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceVisitReportConfiguration());
            modelBuilder.ApplyConfiguration(new SensorLocationConfiguration());
            modelBuilder.ApplyConfiguration(new ElevationAnglesConfiguration());

            //modelBuilder.Entity<ContactPerson>()
            //    .HasOne(_ => _.StationInformation)
            //    .WithMany()
            //    .HasForeignKey(_ => _.ParentGdbArchiveOid)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContactPerson>()
                .HasOne(_ => _.StationInformation)
                .WithMany(_ => _.ContactPersons)
                .HasForeignKey(_ => _.ParentGdbArchiveOid);

            modelBuilder.Entity<ServiceVisitReport>()
                .HasOne(_ => _.StationInformation)
                .WithMany()
                .HasForeignKey(_ => _.ParentGdbArchiveOid)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ElevationAngles>()
                .HasOne(_ => _.SensorLocation)
                .WithMany()
                .HasForeignKey(_ => _.ParentGdbArchiveOid)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}