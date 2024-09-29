using Microsoft.EntityFrameworkCore;
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
        }
    }
}