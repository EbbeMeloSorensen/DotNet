using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;

namespace C2IEDM.Domain.Entities.Geometry.Locations.GeometricVolumes;

public class SurfaceVolume : GeometricVolume
{
    public Guid DefiningSurfaceId { get; set; }
    public Surface DefiningSurface { get; set; } = null!;

    public SurfaceVolume(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}