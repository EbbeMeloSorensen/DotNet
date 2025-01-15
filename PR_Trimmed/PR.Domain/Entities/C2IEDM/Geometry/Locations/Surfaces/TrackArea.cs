using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces
{
    public class TrackArea : Surface
    {
        public Guid BeginPointID { get; set; }
        public Point BeginPoint { get; set; } = null!;

        public Guid EndPointID { get; set; }
        public Point EndPoint { get; set; } = null!;

        public double LeftWidthDimension { get; set; }
        public double RightWidthDimension { get; set; }
    }
}
