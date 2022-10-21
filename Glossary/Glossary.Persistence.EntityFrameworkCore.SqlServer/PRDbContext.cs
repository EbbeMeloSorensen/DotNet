using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.EntityFrameworkCore.SqlServer.EntityConfigurations;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer
{
    public class PRDbContext : DbContext
    {
        public DbSet<Record> People { get; set; }
        public DbSet<RecordAssociation> PersonAssociations { get; set; }

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
            modelBuilder.ApplyConfiguration(new PersonAssociationConfiguration());

            modelBuilder.Entity<RecordAssociation>()
                .HasOne(p => p.SubjectPerson)
                .WithMany(pa => pa.ObjectPeople)
                .HasForeignKey(pa => pa.SubjectPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecordAssociation>()
                .HasOne(p => p.ObjectPerson)
                .WithMany(pa => pa.SubjectPeople)
                .HasForeignKey(pa => pa.ObjectPersonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
