using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using C2IEDM.Domain.Entities.Geometry.Locations.Line;

namespace C2IEDM.Persistence.EntityFrameworkCore.EntityConfigurations
{
    public class LinePointConfiguration : IEntityTypeConfiguration<LinePoint>
    {
        public void Configure(EntityTypeBuilder<LinePoint> builder)
        {
            builder.HasKey(lp => new { lp.LineId, lp.Index });
        }
    }
}
