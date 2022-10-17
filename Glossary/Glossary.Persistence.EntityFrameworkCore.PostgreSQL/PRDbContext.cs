using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class PRDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<PersonAssociation> PersonAssociations { get; set; }

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
            modelBuilder.ApplyConfiguration(new PersonAssociationConfiguration());

            modelBuilder.Entity<PersonAssociation>()
                .HasOne(p => p.SubjectPerson)
                .WithMany(pa => pa.ObjectPeople)
                .HasForeignKey(pa => pa.SubjectPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonAssociation>()
                .HasOne(p => p.ObjectPerson)
                .WithMany(pa => pa.SubjectPeople)
                .HasForeignKey(pa => pa.ObjectPersonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
