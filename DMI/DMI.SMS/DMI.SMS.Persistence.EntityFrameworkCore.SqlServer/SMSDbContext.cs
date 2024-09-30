using Microsoft.EntityFrameworkCore;
using DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations;

namespace DMI.SMS.Persistence.EntityFrameworkCore.SqlServer
{
    public class SMSDbContext : SMSDbContextBase
    {
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}