namespace C2IEDM.Domain.Entities.Geometry.Locations.Points;

public class AbsolutePoint : Point
{
    public double LatitudeCoordinate { get; set; }
    public double LongitudeCoordinate { get; set; }

    public Guid? VerticalDistanceId { get; set; }
    public virtual VerticalDistance? VerticalDistance { get; set; }

    public override List<double> AsListOfDouble()
    {
        return new List<double> { LatitudeCoordinate, LongitudeCoordinate };
    }

    public override string ToString()
    {
        return $"AbsolutePoint (Lat: {LatitudeCoordinate}, Long: {LongitudeCoordinate})";
    }

    public AbsolutePoint(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}