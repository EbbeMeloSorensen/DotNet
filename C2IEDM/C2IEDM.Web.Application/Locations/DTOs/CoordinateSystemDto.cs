namespace C2IEDM.Web.Application.Locations.DTOs;

public class CoordinateSystemDto
{
    public string type { get; set; }
    public Guid id { get; set; }

    public CoordinateSystemDto()
    {
        type = "Coordinate System";
    }
}