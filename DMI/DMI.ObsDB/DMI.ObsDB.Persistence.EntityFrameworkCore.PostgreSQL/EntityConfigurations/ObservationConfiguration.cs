using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations
{
    public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
    {
        public void Configure(EntityTypeBuilder<Observation> builder)
        {
            builder.HasKey(_ => _.Id);
        }
    }
}
