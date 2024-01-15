using WIGOS.Domain.Entities.Geometry.Locations;
using FluentValidation;

namespace WIGOS.Web.Application.Geometry
{
    public class LocationValidator : AbstractValidator<Location>
    {
        public LocationValidator()
        {
            //RuleFor(x => x.FirstName).NotEmpty();
        }
    }
}