namespace C2IEDM.Domain.Entities.Geometry;

public class RelativePoint : Point
{
    public Guid CoordinateSystemId { get; set; }
    public CoordinateSystem CoordinateSystem { get; set; } = null!;

    public double XCoordinateDimension { get; set; }
    public double YCoordinateDimension { get; set; }
    public double ZCoordinateDimension { get; set; }

    public override List<double> AsListOfDouble()
    {
        throw new NotImplementedException();
    }
}