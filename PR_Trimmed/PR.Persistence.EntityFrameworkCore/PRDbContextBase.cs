using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.EntityFrameworkCore.EntityConfigurations;

namespace PR.Persistence.EntityFrameworkCore
{
    public class PRDbContextBase : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<PersonAssociation> PersonAssociations { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new PersonAssociationConfiguration());

            modelBuilder.Entity<PersonAssociation>()
                .HasOne(p => p.SubjectPerson)
                .WithMany(pa => pa.ObjectPeople)
                .HasForeignKey(pa => pa.SubjectPersonId);

            modelBuilder.Entity<PersonAssociation>()
                .HasOne(p => p.ObjectPerson)
                .WithMany(pa => pa.SubjectPeople)
                .HasForeignKey(pa => pa.ObjectPersonId);
        }
    }
}
