using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.PostgreSQL.EntityConfigurations
{
    public class TimeSeriesConfiguration : IEntityTypeConfiguration<TimeSeries>
    {
        public void Configure(EntityTypeBuilder<TimeSeries> builder)
        {
            builder.HasKey(_ => _.Id);
        }
    }
}
