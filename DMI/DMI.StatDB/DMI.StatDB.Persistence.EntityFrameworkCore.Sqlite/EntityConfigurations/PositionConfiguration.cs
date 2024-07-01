using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite.EntityConfigurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.HasKey(p => p.StatID);
        }
    }
}
