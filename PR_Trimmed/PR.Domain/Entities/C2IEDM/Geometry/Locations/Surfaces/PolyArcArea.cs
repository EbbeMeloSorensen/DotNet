using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces
{
    public class PolyArcArea : Surface
    {
        public Guid DefiningLineID { get; set; }
        public Line.Line DefiningLine { get; set; } = null!;

        public Guid BearingOriginPointID { get; set; }
        public Point BearingOriginPoint { get; set; } = null!;

        public double BeginBearingAngle { get; set; }
        public double EndBearingAngle { get; set; }
        public double ArcRadiusDimension { get; set; }
    }
}
