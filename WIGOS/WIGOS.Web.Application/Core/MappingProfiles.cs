using AutoMapper;
using WIGOS.Domain.Entities;
using WIGOS.Domain.Entities.Geometry;
using WIGOS.Domain.Entities.Geometry.CoordinateSystems;
using WIGOS.Domain.Entities.Geometry.Locations;
using WIGOS.Domain.Entities.Geometry.Locations.Line;
using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Domain.Entities.ObjectItems;
using WIGOS.Domain.Entities.ObjectItems.Organisations;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using WIGOS.Web.Application.Geometry.DTOs;
using WIGOS.Web.Application.Geometry.VerticalDistance;
using WIGOS.Web.Application.ObjectItems.DTOs;
using WIGOS.Web.Application.People;
using WIGOS.Web.Application.WIGOS.DTOs;

namespace WIGOS.Web.Application.Core
{
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

            CreateMap<GeospatialLocation, GeospatialLocation>();
            CreateMap<GeospatialLocation, GeospatialLocationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ObjectId))
                .Include<Domain.Entities.WIGOS.GeospatialLocations.Point, WIGOS.DTOs.PointDto>();
            CreateMap<Domain.Entities.WIGOS.GeospatialLocations.Point, WIGOS.DTOs.PointDto>();
        }
    }
}