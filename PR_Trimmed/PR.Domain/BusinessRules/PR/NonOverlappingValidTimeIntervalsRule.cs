using System;
using System.Collections.Generic;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR
{
    public class NonOverlappingValidTimeIntervalsRule : IBusinessRule<IEnumerable<Person>>
    {
        public string RuleName => "NonOverlappingValidTimeIntervals";

        public bool Validate(
            IEnumerable<Person> personVariants)
        {
            throw new NotImplementedException();
        }

        public string ErrorMessage => "Overlapping valid time intervals detected";
    }
}