using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;

namespace PR.Persistence.EntityFrameworkCore.EntityConfigurations
{
    public class PersonAssociationConfiguration : IEntityTypeConfiguration<PersonAssociation>
    {
        public void Configure(EntityTypeBuilder<PersonAssociation> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
