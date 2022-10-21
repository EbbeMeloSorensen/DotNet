using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Glossary.Domain.Entities;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations;

public class PersonAssociationConfiguration : IEntityTypeConfiguration<RecordAssociation>
{
    public void Configure(EntityTypeBuilder<RecordAssociation> builder)
    {
        builder.HasKey(p => p.Id);
    }
}