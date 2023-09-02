using AutoMapper;
using C2IEDM.Domain.Entities;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Web.Application.Locations.DTOs;
using C2IEDM.Web.Application.People;

namespace C2IEDM.Web.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Person, Person>();
        CreateMap<Person, PersonDto>();

        CreateMap<Location, Location>();
        CreateMap<Location, LocationDto>()
            .Include<Point, PointDto>()
            .Include<AbsolutePoint, AbsolutePointDto>()
            .Include<Line, LineDto>();
        CreateMap<Point, PointDto>();
        CreateMap<AbsolutePoint, AbsolutePointDto>();
        CreateMap<Line, LineDto>();
    }
}