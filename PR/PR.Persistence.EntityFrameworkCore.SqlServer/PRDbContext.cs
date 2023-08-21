using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.EntityFrameworkCore.EntityConfigurations;

namespace PR.Persistence.EntityFrameworkCore.SqlServer
{
    public class PRDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
        }
    }
}
