using C2IEDM.Domain.Entities.Geometry.Locations.Points;

namespace C2IEDM.Domain.Entities.Geometry.Locations.GeometricVolumes;

public class SphereVolume : GeometricVolume
{
    public Guid CentrePointId { get; set; }
    public Point CentrePoint { get; set; } = null!;

    public double RadiusDimension { get; set; }
}