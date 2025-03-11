using System;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class StartIsRequiredRule : IBusinessRule<Person>
    {
        public string RuleName => "StartIsRequired";

        public bool Validate(
            Person person)
        {
            return person.Start != default(DateTime);
        }

        public string ErrorMessage => "Start is required";
    }
}
