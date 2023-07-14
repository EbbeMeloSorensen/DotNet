using DMI.ObsDB.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations
{
    public class TimeSeriesConfiguration : IEntityTypeConfiguration<TimeSeries>
    {
        public void Configure(EntityTypeBuilder<TimeSeries> builder)
        {
            builder.HasKey(_ => _.Id);
        }
    }
}
