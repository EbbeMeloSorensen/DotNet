using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations
{
    public class ObservingFacilityConfiguration : IEntityTypeConfiguration<ObservingFacility>
    {
        public void Configure(EntityTypeBuilder<ObservingFacility> builder)
        {
            builder.HasKey(_ => _.Id);
        }
    }
}
