using AutoMapper;
using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Domain.Entities.Geometry.CoordinateSystems;
using C2IEDM.Domain.Entities.Geometry.Locations;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;
using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Application.People;

namespace C2IEDM.Web.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Person, Person>();
        CreateMap<Person, PersonDto>();

        CreateMap<VerticalDistance, VerticalDistance>();
        CreateMap<VerticalDistance, VerticalDistanceDto>();

        CreateMap<CoordinateSystem, CoordinateSystem>();
        CreateMap<CoordinateSystem, CoordinateSystemDto>()
            .Include<PointReference, PointReferenceDto>();
        CreateMap<PointReference, PointReferenceDto>();

        CreateMap<Location, Location>();
        CreateMap<Location, LocationDto>()
            .Include<Point, PointDto>()
            .Include<AbsolutePoint, AbsolutePointDto>()
            .Include<RelativePoint, RelativePointDto>()
            .Include<Line, LineDto>()
            .Include<Surface, SurfaceDto>()
            .Include<Ellipse, EllipseDto>()
            .Include<CorridorArea, CorridorAreaDto>()
            .Include<PolygonArea, PolygonAreaDto>()
            .Include<FanArea, FanAreaDto>();
        CreateMap<Point, PointDto>();
        CreateMap<AbsolutePoint, AbsolutePointDto>();
        CreateMap<RelativePoint, RelativePointDto>();
        CreateMap<Line, LineDto>();
        CreateMap<Surface, SurfaceDto>();
        CreateMap<Ellipse, EllipseDto>();
        CreateMap<CorridorArea, CorridorAreaDto>();
        CreateMap<PolygonArea, PolygonAreaDto>();
        CreateMap<FanArea, FanAreaDto>();
    }
}