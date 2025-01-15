using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;

namespace PR.Domain.Entities.C2IEDM.Geometry.CoordinateSystems
{
    public class PointReference : CoordinateSystem
    {
        public Guid OriginPointID { get; set; }
        public Point OriginPoint { get; set; } = null!;

        public Guid XVectorPointID { get; set; }
        public Point XVectorPoint { get; set; } = null!;

        public Guid YVectorPointID { get; set; }
        public Point YVectorPoint { get; set; } = null!;
    }
}