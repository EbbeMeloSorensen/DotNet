using DMI.SMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;

public class ElevationAnglesConfiguration : IEntityTypeConfiguration<ElevationAngles>
{
    public void Configure(EntityTypeBuilder<ElevationAngles> builder)
    {
        builder.HasKey(_ => _.GdbArchiveOid);
    }
}