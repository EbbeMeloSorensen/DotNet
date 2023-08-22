namespace C2IEDM.Domain.Entities.Geometry;

public class TrackArea : Surface
{
    public Guid BeginPointId { get; set; }
    public Point BeginPoint { get; set; } = null!;

    public Guid EndPointId { get; set; }
    public Point EndPoint { get; set; } = null!;

    public double LeftWidthDimension { get; set; }
    public double RightWidthDimension { get; set; }
}