using System;
using System.Collections.Generic;
using Craft.Domain;
using Craft.Math;

namespace PR.Domain.BusinessRules.PR.CrossEntityRules
{
    public class DateRangeCollectionRule : IBusinessRule<IEnumerable<Tuple<DateTime, DateTime>>>
    {
        public string RuleName => "NoOverlappingValidTimeIntervals";

        public bool Validate(
            IEnumerable<Tuple<DateTime, DateTime>> timeIntervals)
        {
            return !timeIntervals.AnyOverlaps();
        }

        public string ErrorMessage => "Date ranges overlapping";
    }
}