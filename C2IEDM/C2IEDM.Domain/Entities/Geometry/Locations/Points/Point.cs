namespace C2IEDM.Domain.Entities.Geometry.Locations.Points;

public abstract class Point : Location
{
    // Todo: prøv at lave dette som en extension method i stedet
    public abstract List<double> AsListOfDouble();
}