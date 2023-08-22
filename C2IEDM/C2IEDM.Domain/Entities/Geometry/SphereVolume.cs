namespace C2IEDM.Domain.Entities.Geometry;

public class SphereVolume : GeometricVolume
{
    public Guid CentrePointId { get; set; }
    public Point CentrePoint { get; set; } = null!;
    
    public double RadiusDimension { get; set; }
}