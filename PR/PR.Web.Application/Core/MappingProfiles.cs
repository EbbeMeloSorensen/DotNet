﻿using AutoMapper;
using PR.Domain.Entities;
using PR.Web.Application.People;

namespace PR.Web.Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Person, Person>();
        CreateMap<Person, PersonDto>();
    }
}