using C2IEDM.Domain.Entities.Geometry.Locations.Points;

namespace C2IEDM.Domain.Entities.Geometry.Locations.Line;

public class LinePoint : VersionedObject
{
    public Guid LineId { get; set; }
    public Guid LineObjectId { get; set; }
    public Line Line { get; set; } = null!;

    public Guid PointId { get; set; }
    public Guid PointObjectId { get; set; }
    public Point Point { get; set; } = null!;

    public int Index { get; set; }
    public int SequenceQuantity { get; set; }

    public LinePoint(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}