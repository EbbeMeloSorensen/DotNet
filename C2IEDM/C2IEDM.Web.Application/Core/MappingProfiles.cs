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
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using C2IEDM.Web.Application.Geometry.DTOs;
using C2IEDM.Web.Application.Geometry.VerticalDistance;
using C2IEDM.Web.Application.ObjectItems.DTOs;
using C2IEDM.Web.Application.People;
using C2IEDM.Web.Application.WIGOS.DTOs;

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
            .Include<Domain.Entities.Geometry.Locations.Points.Point, Geometry.DTOs.PointDto>()
            .Include<AbsolutePoint, AbsolutePointDto>()
            .Include<RelativePoint, RelativePointDto>()
            .Include<Line, LineDto>()
            .Include<Surface, SurfaceDto>()
            .Include<Ellipse, EllipseDto>()
            .Include<CorridorArea, CorridorAreaDto>()
            .Include<PolygonArea, PolygonAreaDto>()
            .Include<FanArea, FanAreaDto>();
        // Bemærk den her, som vi har for Point, selv om den er en underkategori af Location. Det er for at facilitere, at man kan
        // mappe "polymorfisk" med udgangspunkt i Point, som man f.eks. gør, når man henter punkter for en linie 
        CreateMap<Domain.Entities.Geometry.Locations.Points.Point, Geometry.DTOs.PointDto>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ObjectId))
            .Include<AbsolutePoint, AbsolutePointDto>()
            .Include<RelativePoint, RelativePointDto>();
        CreateMap<AbsolutePoint, AbsolutePointDto>();
        CreateMap<RelativePoint, RelativePointDto>();
        CreateMap<Line, LineDto>();
        CreateMap<Surface, SurfaceDto>();
        CreateMap<Ellipse, EllipseDto>();
        CreateMap<CorridorArea, CorridorAreaDto>();
        CreateMap<PolygonArea, PolygonAreaDto>();
        CreateMap<FanArea, FanAreaDto>();

        CreateMap<AbstractEnvironmentalMonitoringFacility, AbstractEnvironmentalMonitoringFacility>();
        CreateMap<AbstractEnvironmentalMonitoringFacility, AbstractEnvironmentalMonitoringFacilityDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ObjectId))
            .Include<ObservingFacility, ObservingFacilityDto>();
        CreateMap<ObservingFacility, ObservingFacilityDto>();

        CreateMap<GeoSpatialLocation, GeoSpatialLocation>();
        CreateMap<GeoSpatialLocation, GeospatialLocationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ObjectId))
            .Include<Domain.Entities.WIGOS.GeospatialLocations.Point, WIGOS.DTOs.PointDto>();
        CreateMap<Domain.Entities.WIGOS.GeospatialLocations.Point, WIGOS.DTOs.PointDto>();
    }
}