using DMI.SMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
    }
}