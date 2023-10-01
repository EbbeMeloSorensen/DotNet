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

    public override string ToString()
    {
        var result = $"({Coordinate1}, {Coordinate2})";

        if (From.HasValue || To.HasValue)
        {
            result += ",";
        }

        if (From.HasValue)
        {
            result += $" From: {From.Value.ToShortDateString()}";
        }

        if (To.HasValue)
        {
            result += $" To: {To.Value.ToShortDateString()}";
        }

        return result;
    }
}