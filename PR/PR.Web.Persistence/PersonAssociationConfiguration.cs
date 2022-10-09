using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PR.Domain.Entities;

namespace PR.Web.Persistence;

public class PersonAssociationConfiguration : IEntityTypeConfiguration<PersonAssociation>
{
    public void Configure(EntityTypeBuilder<PersonAssociation> builder)
    {
        builder.HasKey(p => p.Id);
    }
}