using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.GeometricVolumes
{
    public class SphereVolume : GeometricVolume
    {
        public Guid CentrePointID { get; set; }
        public Point CentrePoint { get; set; } = null!;

        public double RadiusDimension { get; set; }
    }
}
