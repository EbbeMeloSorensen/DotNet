using PR.Domain;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace PR.Persistence.Versioned
{
    internal static class Helpers
    {
        internal static void AddVersionPredicates<T>(
            this ICollection<Expression<Func<T, bool>>> predicates,
            DateTime? databaseTime) where T : IVersionedObject
        {
            if (databaseTime.HasValue)
            {
                predicates.Add(pa =>
                    pa.Created <= databaseTime &&
                    pa.Superseded > databaseTime);
            }
            else
            {
                predicates.Add(pa =>
                    pa.Superseded.Year == 9999);
            }
        }

        internal static void AddHistoryPredicates<T>(
            this ICollection<Expression<Func<T, bool>>> predicates,
            DateTime? historicalTime,
            bool includeHistoricalObjects,
            bool includeCurrentObjects) where T : IObjectWithValidTime
        {
            historicalTime ??= DateTime.UtcNow;

            if (includeHistoricalObjects)
            {
                predicates.Add(p => p.Start <= historicalTime);
            }
            else if (includeCurrentObjects)
            {
                // ONLY current objects
                predicates.Add(p => p.Start <= historicalTime && p.End > historicalTime);
            }
            else
            {
                throw new InvalidOperationException("Either Include current or include historical should be true");
            }
        }
    }
}
