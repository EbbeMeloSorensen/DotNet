using DMI.ObsDB.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations
{
    public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
    {
        public void Configure(EntityTypeBuilder<Observation> builder)
        {
            builder.HasKey(_ => _.Id);
        }
    }
}
