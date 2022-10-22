using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Glossary.Domain.Entities;

namespace Glossary.Web.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Record> Records { get; set; }
        public DbSet<RecordAssociation> RecordAssociations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new RecordConfiguration());
            builder.ApplyConfiguration(new RecordAssociationConfiguration());

            builder.Entity<RecordAssociation>()
                .HasOne(p => p.SubjectRecord)
                .WithMany(pa => pa.ObjectRecords)
                .HasForeignKey(pa => pa.SubjectRecordId);

            builder.Entity<RecordAssociation>()
                .HasOne(p => p.ObjectRecord)
                .WithMany(pa => pa.SubjectRecords)
                .HasForeignKey(pa => pa.ObjectRecordId);

            base.OnModelCreating(builder);
        }
    }
}
