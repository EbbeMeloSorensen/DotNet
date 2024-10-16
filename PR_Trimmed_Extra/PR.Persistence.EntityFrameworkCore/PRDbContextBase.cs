using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.EntityFrameworkCore.EntityConfigurations;

namespace PR.Persistence.EntityFrameworkCore
{
    public class PRDbContextBase : DbContext
    {
        public static bool Versioned { get; set; }

        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration(Versioned));
        }
    }
}
