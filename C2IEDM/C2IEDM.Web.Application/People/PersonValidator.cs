using C2IEDM.Domain.Entities;
using FluentValidation;

namespace C2IEDM.Web.Application.People;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
    }
}