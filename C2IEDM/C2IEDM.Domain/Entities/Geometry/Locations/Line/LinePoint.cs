using C2IEDM.Domain.Entities.Geometry.Locations.Points;

namespace C2IEDM.Domain.Entities.Geometry.Locations.Line;

public class LinePoint
{
    public Guid LineId { get; set; }
    public Line Line { get; set; } = null!;

    public Guid PointId { get; set; }
    public Point Point { get; set; } = null!;

    public int Index { get; set; }
    public int SequenceQuantity { get; set; }
}