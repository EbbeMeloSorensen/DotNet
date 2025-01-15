using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Points;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.GeometricVolumes
{
    public class ConeVolume : GeometricVolume
    {
        public Guid DefiningSurfaceID { get; set; }
        public Surface DefiningSurface { get; set; } = null!;

        public Guid VertexPointID { get; set; }
        public Point VertexPoint { get; set; } = null!;
    }
}
