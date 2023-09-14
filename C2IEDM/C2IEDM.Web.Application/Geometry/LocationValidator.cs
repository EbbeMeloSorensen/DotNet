using C2IEDM.Domain.Entities.Geometry.Locations;
using FluentValidation;

namespace C2IEDM.Web.Application.Geometry;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        //RuleFor(x => x.FirstName).NotEmpty();
    }
}