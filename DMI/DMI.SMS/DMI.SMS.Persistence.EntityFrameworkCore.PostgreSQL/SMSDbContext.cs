using Microsoft.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations;

namespace DMI.SMS.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class SMSDbContext : SMSDbContextBase
    {
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(
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