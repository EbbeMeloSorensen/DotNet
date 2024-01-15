using WIGOS.Domain.Entities;
using FluentValidation;

namespace WIGOS.Web.Application.People
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
        }
    }
}