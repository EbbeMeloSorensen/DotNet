using Craft.Domain;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class CityIsValidRule : IBusinessRule<Person>
    {
        public string RuleName => "City";

        public string ErrorMessage { get; private set; }

        public bool Validate(
            Person person)
        {
            if (!string.IsNullOrEmpty(person.City) && person.City.Length > 10)
            {
                ErrorMessage = "City too long (max 10 characters)";
                return false;
            }

            return true;
        }
    }
}