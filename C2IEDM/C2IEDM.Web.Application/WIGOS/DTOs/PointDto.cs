namespace C2IEDM.Web.Application.WIGOS.DTOs;

public class PointDto : GeospatialLocationDto
{
    public string CoordinateSystem { get; set; }
    public double Coordinate1 { get; set; }
    public double Coordinate2 { get; set; }

    public PointDto()
    {
        Category = "Point";
    }
}
