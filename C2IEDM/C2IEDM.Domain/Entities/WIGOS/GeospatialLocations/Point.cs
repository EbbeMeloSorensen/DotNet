namespace C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

public class Point : GeospatialLocation
{
    public string CoordinateSystem { get; set; }
    public double Coordinate1 { get; set; }
    public double Coordinate2 { get; set; }

    public Point(
        Guid objectId, 
        DateTime created) : base(objectId, created)
    {
    }
}