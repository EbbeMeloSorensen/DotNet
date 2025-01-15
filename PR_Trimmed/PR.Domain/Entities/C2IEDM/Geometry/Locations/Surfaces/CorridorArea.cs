using System;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces
{
    public class CorridorArea : Surface
    {
        public Guid CenterLineID { get; set; }
        public Line.Line CenterLine { get; set; } = null!;

        public double WidthDimension { get; set; }
    }
}
