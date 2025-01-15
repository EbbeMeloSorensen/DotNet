using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces
{
    public class FanArea : Surface
    {
        public Guid VertexPointID { get; set; }
        public Point VertexPoint { get; set; } = null!;

        public double MinimumRangeDimension { get; set; }
        public double MaximumRangeDimension { get; set; }
        public double OrientationAngle { get; set; }
        public double SectorSizeAngle { get; set; }
    }
}
