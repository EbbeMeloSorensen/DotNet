namespace C2IEDM.Domain.Entities.Geometry;

public class SurfaceVolume : GeometricVolume
{
    public Guid DefiningSurfaceId { get; set; }
    public Surface DefiningSurface { get; set; } = null!;
}