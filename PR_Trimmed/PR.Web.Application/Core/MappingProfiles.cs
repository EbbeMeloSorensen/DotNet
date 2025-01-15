using AutoMapper;
using PR.Domain.Entities.Smurfs;
using PR.Domain.Entities.PR;
using PR.Web.Application.Smurfs;
using PR.Web.Application.People;

namespace PR.Web.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Smurf, SmurfDto>();

        CreateMap<Person, Person>();
        CreateMap<Person, PersonDto>();
    }
}