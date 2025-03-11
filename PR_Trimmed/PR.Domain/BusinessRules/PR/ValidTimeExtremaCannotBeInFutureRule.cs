using System;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class ValidTimeExtremaCannotBeInFutureRule : IBusinessRule<Person>
    {
        public string RuleName => "ValidTimeExtremaCannotBeInFuture";

        public bool Validate(
            Person person)
        {
            var result =
                person.Start <= DateTime.UtcNow &&
                (person.End.Year == 9999 || person.End <= DateTime.UtcNow);

            return result;
        }

        public string ErrorMessage => "Valid Time extrema cannot be in the future";
    }
}
