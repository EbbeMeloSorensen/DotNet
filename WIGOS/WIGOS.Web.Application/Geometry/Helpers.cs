using WIGOS.Domain.Entities.Geometry.Locations;
using WIGOS.Domain.Entities.Geometry.Locations.Line;
using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Web.Application.Geometry.DTOs;

namespace WIGOS.Web.Application.Geometry
{
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
}