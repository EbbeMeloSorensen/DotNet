using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Glossary.Domain.Entities;

namespace Glossary.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations;

public class PersonAssociationConfiguration : IEntityTypeConfiguration<PersonAssociation>
{
    public void Configure(EntityTypeBuilder<PersonAssociation> builder)
    {
        builder.HasKey(p => p.Id);
    }
}