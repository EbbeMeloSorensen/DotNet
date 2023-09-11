using C2IEDM.Domain.Entities.Geometry.Locations.Points;

namespace C2IEDM.Domain.Entities.Geometry.CoordinateSystems;

public class PointReference : CoordinateSystem
{
    public Guid OriginPointId { get; set; }
    public Point OriginPoint { get; set; } = null!;

    public Guid XVectorPointId { get; set; }
    public Point XVectorPoint { get; set; } = null!;

    public Guid YVectorPointId { get; set; }
    public Point YVectorPoint { get; set; } = null!;

    public PointReference(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}