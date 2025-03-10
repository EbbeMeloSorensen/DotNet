using System;
using System.Collections.Generic;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class NoOverlappingValidTimeIntervalsRule : IBusinessRule<IEnumerable<Person>>
    {
        public string RuleName => "NoOverlappingValidTimeIntervals";

        public bool Validate(
            IEnumerable<Person> personVariants)
        {
            throw new NotImplementedException();
        }

        public string ErrorMessage => "Overlapping valid time intervals detected";
    }
}