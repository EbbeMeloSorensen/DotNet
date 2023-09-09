using C2IEDM.Domain.Entities.Geometry.Locations.Points;

namespace C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;

public class TrackArea : Surface
{
    public Guid BeginPointId { get; set; }
    public Point BeginPoint { get; set; } = null!;

    public Guid EndPointId { get; set; }
    public Point EndPoint { get; set; } = null!;

    public double LeftWidthDimension { get; set; }
    public double RightWidthDimension { get; set; }
}