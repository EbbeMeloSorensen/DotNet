using DMI.StatDB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.SqlServer.EntityConfigurations
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(
            EntityTypeBuilder<Station> builder)
        {
            builder.HasKey(s => s.StatID);

            builder.Property(s => s.StatID).HasColumnName("statid");
            builder.Property(s => s.IcaoId).HasColumnName("icao_id").IsRequired(false);
            builder.Property(s => s.Country).HasColumnName("country").IsRequired(false);
            builder.Property(s => s.Source).HasColumnName("source").IsRequired(false);
        }
    }
}
