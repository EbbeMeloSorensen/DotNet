using System;
using PR.Domain.Entities.C2IEDM.Geometry.Locations.Surfaces;

namespace PR.Domain.Entities.C2IEDM.Geometry.Locations.GeometricVolumes
{
    public class SurfaceVolume : GeometricVolume
    {
        public Guid DefiningSurfaceID { get; set; }
        public Surface DefiningSurface { get; set; } = null!;
    }
}
