namespace C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;

public class PolygonArea : Surface
{
    public Guid BoundingLineId { get; set; }
    public Line.Line BoundingLine { get; set; } = null!;
}