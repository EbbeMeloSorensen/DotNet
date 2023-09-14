namespace C2IEDM.Web.Application.Geometry.DTOs;

public class CorridorAreaDto : SurfaceDto
{
    public List<PointDto> CenterLinePoints { get; set; }

    public double WidthDimension { get; set; }

    public CorridorAreaDto()
    {
        type = "Corridor Area";
    }
}