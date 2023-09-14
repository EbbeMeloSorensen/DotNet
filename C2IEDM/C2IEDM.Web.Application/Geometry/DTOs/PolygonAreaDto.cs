namespace C2IEDM.Web.Application.Geometry.DTOs;

public class PolygonAreaDto : SurfaceDto
{
    public List<PointDto> BoundingLinePoints { get; set; }

    public PolygonAreaDto()
    {
        type = "Polygon Area";
    }
}