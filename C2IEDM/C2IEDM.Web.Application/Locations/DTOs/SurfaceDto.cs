namespace C2IEDM.Web.Application.Locations.DTOs;

public class SurfaceDto : LocationDto
{
    public string type { get; set; }

    public SurfaceDto()
    {
        type = "Surface";
    }
}