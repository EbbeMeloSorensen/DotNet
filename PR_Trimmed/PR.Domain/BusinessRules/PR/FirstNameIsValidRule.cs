using Craft.Domain;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class FirstNameIsValidRule : IBusinessRule<Person>
    {
        public string RuleName => "FirstName";

        public string ErrorMessage { get; private set; }

        public bool Validate(
            Person person)
        {
            if (string.IsNullOrEmpty(person.FirstName))
            {
                ErrorMessage = "Required";
                return false;
            }
            
            if (person.FirstName.Length > 20)
            {
                ErrorMessage = "Too long";
                return false;
            }

            return true;
        }
    }
}