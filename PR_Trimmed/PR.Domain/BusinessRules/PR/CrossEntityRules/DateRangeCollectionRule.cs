using System;
using System.Collections.Generic;
using System.Linq;
using Craft.Domain;
using Craft.Math;
using PR.Domain.Entities.PR;

namespace PR.Domain.BusinessRules.PR.CrossEntityRules
{
    public class DateRangeCollectionRule : IBusinessRule<IEnumerable<Person>>
    {
        public string RuleName => "NoOverlappingValidTimeIntervals";

        public bool Validate(
            IEnumerable<Person> personVariants)
        {
            return !personVariants.Select(_ => new Tuple<DateTime, DateTime>(_.Start, _.End)).AnyOverlaps();
        }

        public string ErrorMessage => "Date ranges overlapping";
    }
}