using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations;

namespace PR.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class PRDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
        }

    }
}
