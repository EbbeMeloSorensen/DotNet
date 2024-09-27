using Microsoft.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations;
using DMI.SMS.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite
{
    public class SMSDbContext : SMSDbContextBase
    {
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