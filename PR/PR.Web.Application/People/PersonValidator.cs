using FluentValidation;
using PR.Domain.Entities;

namespace PR.Web.Application.People;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
    }
}