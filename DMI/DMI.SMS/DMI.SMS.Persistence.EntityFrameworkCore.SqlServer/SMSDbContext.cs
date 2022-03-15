using Microsoft.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.EntityFrameworkCore.SqlServer.EntityConfigurations;

namespace DMI.SMS.Persistence.EntityFrameworkCore.SqlServer
{
    public class SMSDbContext : DbContext
    {
        public DbSet<StationInformation> StationInformations { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StationInformationConfiguration());
        }
    }
}