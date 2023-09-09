using C2IEDM.Domain.Entities.Geometry.Locations;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;
using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;
using C2IEDM.Web.Application.Locations.DTOs;

namespace C2IEDM.Web.Application.Locations;

public static class Helpers
{
    public static LocationDto AsLocationDto(
        this Location location)
    {
        switch (location)
        {
            case FanArea fanArea:
            {
                return new FanAreaDto()
                {
                    id = fanArea.Id
                };
            }
            case PolygonArea polygonArea:
            {
                return new PolygonAreaDto()
                {
                    id = polygonArea.Id
                };
            }
            case CorridorArea corridorArea:
            {
                return new CorridorAreaDto()
                {
                    id = corridorArea.Id
                };
            }
            case Ellipse ellipse:
            {
                return new EllipseDto()
                {
                    id = ellipse.Id
                };
            }
            case AbsolutePoint absolutePoint:
            {
                return new AbsolutePointDto()
                {
                    id = absolutePoint.Id,
                    latitudeCoordinate = absolutePoint.LatitudeCoordinate,
                    longitudeCoordinate = absolutePoint.LongitudeCoordinate
                };
            }
            case Line line:
            {
                return new LineDto()
                {
                    id = line.Id
                };
            }
            default:
            {
                throw new NotImplementedException();
            }
        }
    }
}