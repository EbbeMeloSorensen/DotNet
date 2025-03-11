using Craft.Domain;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class EndMustBeLaterThanStartRule : IBusinessRule<Person>
    {
        public string RuleName => "EndMustBeLaterThanStart";

        public bool Validate(
            Person person)
        {
            return person.Start < person.End;
        }

        public string ErrorMessage => "End time must be later than start time";
    }
}