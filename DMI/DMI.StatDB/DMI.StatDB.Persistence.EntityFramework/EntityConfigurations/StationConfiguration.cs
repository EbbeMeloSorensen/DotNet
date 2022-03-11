using System.Data.Entity.ModelConfiguration;
using DMI.StatDB.Domain.Entities;

namespace DMI.StatDB.Persistence.EntityFramework.EntityConfigurations
{
    public class StationConfiguration : EntityTypeConfiguration<Station>
    {
        public StationConfiguration()
        {
            ToTable("station");
            HasKey(s => s.StatID);
            Property(s => s.Country).HasColumnName("country");
            Property(s => s.IcaoId).HasColumnName("icao_id");
            Property(s => s.Source).HasColumnName("source");
        }
    }
}
