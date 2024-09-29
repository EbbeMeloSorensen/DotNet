using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations;

public class ElevationAnglesConfiguration : IEntityTypeConfiguration<ElevationAngles>
{
    public void Configure(EntityTypeBuilder<ElevationAngles> builder)
    {
        builder.HasKey(_ => _.GdbArchiveOid);
    }
}