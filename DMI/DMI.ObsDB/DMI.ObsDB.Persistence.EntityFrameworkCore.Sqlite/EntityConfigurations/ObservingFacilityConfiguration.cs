using DMI.ObsDB.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations
{
    public class ObservingFacilityConfiguration : IEntityTypeConfiguration<ObservingFacility>
    {
        public void Configure(EntityTypeBuilder<ObservingFacility> builder)
        {
            builder.HasKey(_ => _.Id);
        }
    }
}
