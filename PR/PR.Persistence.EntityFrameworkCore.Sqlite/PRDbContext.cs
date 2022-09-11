using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;

namespace PR.Persistence.EntityFrameworkCore.Sqlite
{
    public class PRDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
        }
    }
}
