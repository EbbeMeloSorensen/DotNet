using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class FirstNameIsRequiredRule : IBusinessRule<Person>
    {
        public string RuleName => "FirstNameIsRequired";

        public bool Validate(
            Person person)
        {
            return !string.IsNullOrEmpty(person.FirstName);
        }

        public string ErrorMessage => "First name is required";
    }
}