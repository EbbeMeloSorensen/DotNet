namespace C2IEDM.Web.Application.Locations.DTOs;

public class LineDto : LocationDto
{
    public string type { get; set; }
    public Guid id { get; set; }

    public LineDto()
    {
        type = "Line";
    }
}