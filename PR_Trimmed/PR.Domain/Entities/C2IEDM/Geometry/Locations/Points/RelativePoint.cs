using System;
using PR.Domain.Entities.C2IEDM.Geometry.CoordinateSystems;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Points
{
    public class RelativePoint : Point
    {
        public Guid CoordinateSystemID { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; } = null!;

        public double XCoordinateDimension { get; set; }
        public double YCoordinateDimension { get; set; }
        public double ZCoordinateDimension { get; set; }
    }
}
