namespace C2IEDM.Web.Application.Geometry.DTOs;

public class FanAreaDto : SurfaceDto
{
    public PointDto VertexPoint { get; set; }
    public double MinimumRangeDimension { get; set; }
    public double MaximumRangeDimension { get; set; }
    public double OrientationAngle { get; set; }
    public double SectorSizeAngle { get; set; }

    public FanAreaDto()
    {
        type = "Fan Area";
    }
}