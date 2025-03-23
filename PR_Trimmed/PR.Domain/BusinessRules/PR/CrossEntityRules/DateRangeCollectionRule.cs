using System;
using System.Collections.Generic;
using Craft.Domain;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR.CrossEntityRules
{
    public class DateRangeCollectionRule : IBusinessRule<IEnumerable<Person>>
    {
        public string RuleName => "NoOverlappingValidTimeIntervals";

        public bool Validate(
            IEnumerable<Person> personVariants)
        {
            throw new NotImplementedException();
        }

        public string ErrorMessage => "Date ranges overlapping";
    }
}