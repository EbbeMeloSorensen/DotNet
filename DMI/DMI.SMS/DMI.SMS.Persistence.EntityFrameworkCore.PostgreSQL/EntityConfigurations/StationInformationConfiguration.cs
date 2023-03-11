using Microsoft.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMI.SMS.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations
{
    public class StationInformationConfiguration : IEntityTypeConfiguration<StationInformation>
    {
        public void Configure(EntityTypeBuilder<StationInformation> builder)
        {
            builder.HasKey(_ => _.GdbArchiveOid);
        }
    }
}
