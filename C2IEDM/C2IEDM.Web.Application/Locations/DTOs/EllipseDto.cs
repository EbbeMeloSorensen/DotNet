namespace C2IEDM.Web.Application.Locations.DTOs;

public class EllipseDto : SurfaceDto
{
    public PointDto CentrePoint { get; set; }
    public PointDto FirstConjugateDiameterPoint { get; set; }
    public PointDto SecondConjugateDiameterPoint { get; set; }

    public EllipseDto()
    {
        type = "Ellipse";
    }
}