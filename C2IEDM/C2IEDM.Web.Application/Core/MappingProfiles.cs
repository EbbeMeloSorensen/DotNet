using AutoMapper;
using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Domain.Entities.Geometry.CoordinateSystems;
using C2IEDM.Domain.Entities.Geometry.Locations;
using C2IEDM.Domain.Entities.Geometry.Locations.Line;
using C2IEDM.Domain.Entities.Geometry.Locations.Points;
using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;
using C2IEDM.Domain.Entities.ObjectItems;
using C2IEDM.Domain.Entities.ObjectItems.Organisations;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Application.Locations.VerticalDistance;
using C2IEDM.Web.Application.ObjectItems.DTOs;
using C2IEDM.Web.Application.People;

namespace C2IEDM.Web.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Person, Person>();
        CreateMap<Person, PersonDto>();

        CreateMap<ObjectItem, ObjectItem>();
        CreateMap<ObjectItem, ObjectItemDto>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ObjectId))
            .Include<Organisation, OrganisationDto>()
            .Include<Unit, UnitDto>();
        CreateMap<Organisation, OrganisationDto>();
        CreateMap<Unit, UnitDto>();

        CreateMap<VerticalDistance, VerticalDistance>();
        CreateMap<VerticalDistance, VerticalDistanceDto>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ObjectId));

        CreateMap<CoordinateSystem, CoordinateSystem>();
        CreateMap<CoordinateSystem, CoordinateSystemDto>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ObjectId))
            .Include<PointReference, PointReferenceDto>();
        CreateMap<PointReference, PointReferenceDto>();

        CreateMap<Location, Location>();
        CreateMap<Location, LocationDto>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ObjectId))
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