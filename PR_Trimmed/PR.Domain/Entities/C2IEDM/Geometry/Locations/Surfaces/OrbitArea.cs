using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces
{
    public enum OrbitAreaAlignmentCode
    {
        Centre,
        Left,
        Right
    }

    public class OrbitArea : Surface
    {
        public Guid FirstPointID { get; set; }
        public Point FirstPoint { get; set; } = null!;

        public Guid SecondPointID { get; set; }
        public Point SecondPoint { get; set; } = null!;

        public OrbitAreaAlignmentCode OrbitAreaAlignmentCode { get; set; }
        public double WidthDimension { get; set; }
    }
}