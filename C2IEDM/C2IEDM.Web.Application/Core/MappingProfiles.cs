using AutoMapper;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Web.Application.Locations.DTOs;

namespace C2IEDM.Web.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Location, Location>();
        CreateMap<Location, LocationDto>()
            .Include<Point, PointDto>()
            .Include<AbsolutePoint, AbsolutePointDto>();
        CreateMap<Point, PointDto>();
        CreateMap<AbsolutePoint, AbsolutePointDto>();
    }
}