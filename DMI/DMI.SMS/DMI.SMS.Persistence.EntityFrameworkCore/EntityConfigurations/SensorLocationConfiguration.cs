using DMI.SMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations;

public class SensorLocationConfiguration : IEntityTypeConfiguration<SensorLocation>
{
    public void Configure(EntityTypeBuilder<SensorLocation> builder)
    {
        builder.HasKey(_ => _.GdbArchiveOid);
    }
}