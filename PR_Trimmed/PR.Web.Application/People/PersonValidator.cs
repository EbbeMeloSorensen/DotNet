using FluentValidation;
using PR.Domain.Entities.PR;

namespace PR.Web.Application.People;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
    }
}