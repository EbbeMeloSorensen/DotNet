using C2IEDM.Domain.Entities.Geometry.Locations.Points;

namespace C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;

public class FanArea : Surface
{
    public Guid VertexPointId { get; set; }
    public Point VertexPoint { get; set; } = null!;

    public double MinimumRangeDimension { get; set; }
    public double MaximumRangeDimension { get; set; }
    public double OrientationAngle { get; set; }
    public double SectorSizeAngle { get; set; }

    public FanArea(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}