using DMI.StatDB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.EntityConfigurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.HasKey(p => new { p.StatID, p.StartTime, p.Entity });

            builder.Property(p => p.StatID).HasColumnName("statid");
            builder.Property(p => p.Entity).HasColumnName("entity");
            builder.Property(p => p.StartTime).HasColumnName("starttime");
            builder.Property(p => p.EndTime).HasColumnName("endtime").IsRequired(false);
            builder.Property(p => p.Lat).HasColumnName("lat").IsRequired(false);
            builder.Property(p => p.Long).HasColumnName("long").IsRequired(false);
            builder.Property(p => p.Height).HasColumnName("height").IsRequired(false);
        }
    }
}
