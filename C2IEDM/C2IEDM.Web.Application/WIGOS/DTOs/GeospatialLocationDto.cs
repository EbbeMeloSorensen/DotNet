namespace C2IEDM.Web.Application.WIGOS.DTOs;

public class GeospatialLocationDto
{
    public string Category { get; set; }
    public Guid Id { get; set; }

    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public GeospatialLocationDto()
    {
    }
}
