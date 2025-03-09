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
            return person.Start <= DateTime.UtcNow && 
                   person.End <= DateTime.UtcNow;
        }

        public string ErrorMessage => "Valid Time extrema cannot be in the future";
    }
}
