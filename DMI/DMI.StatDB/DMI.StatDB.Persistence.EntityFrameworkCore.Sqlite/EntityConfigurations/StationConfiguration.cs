using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.HasKey(_ => _.StatID);
        }
    }
}
