using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.EntityFrameworkCore.SqlServer.EntityConfigurations;

namespace Glossary.Persistence.EntityFrameworkCore.SqlServer
{
    public class GlossaryDbContext : DbContext
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
            modelBuilder.ApplyConfiguration(new RecordConfiguration());
            modelBuilder.ApplyConfiguration(new RecordAssociationConfiguration());

            modelBuilder.Entity<RecordAssociation>()
                .HasOne(p => p.SubjectRecord)
                .WithMany(pa => pa.ObjectRecords)
                .HasForeignKey(pa => pa.SubjectRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecordAssociation>()
                .HasOne(p => p.ObjectRecord)
                .WithMany(pa => pa.SubjectRecords)
                .HasForeignKey(pa => pa.ObjectRecordId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
