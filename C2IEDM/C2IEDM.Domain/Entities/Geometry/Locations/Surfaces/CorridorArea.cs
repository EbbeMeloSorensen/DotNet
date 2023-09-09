namespace C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;

public class CorridorArea : Surface
{
    public Guid CenterLineId { get; set; }
    public Line CenterLine { get; set; } = null!;

    public double WidthDimension { get; set; }
}