using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PR.Domain.Entities;

namespace PR.Web.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<PersonAssociation> PersonAssociations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PersonConfiguration());
            builder.ApplyConfiguration(new PersonAssociationConfiguration());

            builder.Entity<PersonAssociation>()
                .HasOne(p => p.SubjectPerson)
                .WithMany(pa => pa.ObjectPeople)
                .HasForeignKey(pa => pa.SubjectPersonId);

            builder.Entity<PersonAssociation>()
                .HasOne(p => p.ObjectPerson)
                .WithMany(pa => pa.SubjectPeople)
                .HasForeignKey(pa => pa.ObjectPersonId);

            base.OnModelCreating(builder);
        }
    }
}