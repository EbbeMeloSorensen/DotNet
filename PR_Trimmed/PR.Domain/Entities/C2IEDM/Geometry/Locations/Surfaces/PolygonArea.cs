using System;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces
{
    public class PolygonArea : Surface
    {
        public Guid BoundingLineID { get; set; }
        public Line.Line BoundingLine { get; set; } = null!;
    }
}
