using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities.PR;
using PR.Persistence.EntityFrameworkCore.EntityConfigurations;

namespace PR.Persistence.EntityFrameworkCore
{
    public class PRDbContextBase : DbContext
    {
        public static bool Versioned { get; set; }

        // This constructor is necessary when making a migration with the Package Manager console, since we don't set the static property Versioned when
        // making a migration
        static PRDbContextBase()
        {
            Versioned = true;
        }

        public DbSet<Person> People { get; set; }
        public DbSet<PersonComment> PersonComments { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration(Versioned));
            modelBuilder.ApplyConfiguration(new PersonCommentConfiguration(Versioned));

            // Hvis versioned er true, skal der være en fk reference fra PersonArchiveID i PersonComments-tabellen til ArchiveID i People-tabellen

            if (Versioned)
            {
                modelBuilder.Entity<PersonComment>()
                    .HasOne(pc => pc.Person)
                    .WithMany()
                    .HasForeignKey(pc => pc.PersonArchiveID)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            }
            else
            {
                modelBuilder.Entity<PersonComment>()
                    .HasOne(pc => pc.Person)
                    .WithMany()
                    .HasForeignKey(pc => pc.PersonID)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
