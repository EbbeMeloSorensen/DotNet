﻿using FluentValidation;
using C2IEDM.Domain.Entities.Geometry.Locations;

namespace C2IEDM.Web.Application.Locations;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        //RuleFor(x => x.FirstName).NotEmpty();
    }
}