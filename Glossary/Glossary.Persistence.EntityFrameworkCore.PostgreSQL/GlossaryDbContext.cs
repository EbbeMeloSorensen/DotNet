﻿using Microsoft.EntityFrameworkCore;
using Glossary.Domain.Entities;
using Glossary.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL
{
    public class GlossaryDbContext : DbContext
    {
        public DbSet<Record> Records { get; set; }
        public DbSet<RecordAssociation> RecordAssociations { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString();
            optionsBuilder.UseNpgsql(connectionString);
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
