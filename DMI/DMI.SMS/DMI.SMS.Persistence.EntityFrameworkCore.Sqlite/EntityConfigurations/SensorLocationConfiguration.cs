using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations;

public class SensorLocationConfiguration : IEntityTypeConfiguration<SensorLocation>
{
    public void Configure(EntityTypeBuilder<SensorLocation> builder)
    {
        builder.HasKey(_ => _.GdbArchiveOid);
    }
}