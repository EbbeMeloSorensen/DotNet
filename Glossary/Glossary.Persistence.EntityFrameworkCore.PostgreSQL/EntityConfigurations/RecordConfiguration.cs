﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Glossary.Domain.Entities;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations
{
    public class RecordConfiguration : IEntityTypeConfiguration<Record>
    {
        public void Configure(EntityTypeBuilder<Record> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
