using WIGOS.Domain.Entities.Geometry.Locations.Line;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WIGOS.Persistence.EntityFrameworkCore.EntityConfigurations
{
    public class LinePointConfiguration : IEntityTypeConfiguration<LinePoint>
    {
        public void Configure(EntityTypeBuilder<LinePoint> builder)
        {
            builder.HasKey(lp => new { lp.LineId, lp.Index });
        }
    }
}
