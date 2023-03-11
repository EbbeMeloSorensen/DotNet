using DMI.SMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations
{
    public class StationInformationConfiguration : IEntityTypeConfiguration<StationInformation>
    {
        public void Configure(EntityTypeBuilder<StationInformation> builder)
        {
            builder.HasKey(_ => _.GdbArchiveOid);
        }
    }
}
