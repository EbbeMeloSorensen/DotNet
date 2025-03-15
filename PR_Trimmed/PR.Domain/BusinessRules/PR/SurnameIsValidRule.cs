using Craft.Domain;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class SurnameIsValidRule : IBusinessRule<Person>
    {
        public string RuleName => "Surname";

        public string ErrorMessage { get; private set; }

        public bool Validate(
            Person person)
        {
            if (!string.IsNullOrEmpty(person.Surname) && person.Surname.Length > 10)
            {
                ErrorMessage = "Too long (max 10 characters)";
                return false;
            }

            return true;
        }
    }
}