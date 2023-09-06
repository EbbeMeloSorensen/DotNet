namespace C2IEDM.Web.Application.Locations.DTOs;

public class LineDto : LocationDto
{
    public List<PointDto> LinePoints { get; set; }

    public LineDto()
    {
        type = "Line";
    }
}