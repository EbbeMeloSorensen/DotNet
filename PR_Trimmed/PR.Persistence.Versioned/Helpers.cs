using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;
using Craft.Domain;
using Craft.Math;

namespace PR.Persistence.Versioned
{
    public static class Helpers2
    {
        public static void InsertNewVariant<T>(
            this IEnumerable<T> entities,
            T newEntity,
            out List<T> nonConflictingEntities,
            out List<T> coveredEntities) where T : IObjectWithValidTime
        {
            nonConflictingEntities = new List<T>();
            coveredEntities = new List<T>();

            var newInterval = new Tuple<DateTime, DateTime>(newEntity.Start, newEntity.End);

            foreach (var entity in entities)
            {
                var otherInterval = new Tuple<DateTime, DateTime>(entity.Start, entity.End);

                if (!newInterval.Overlaps(otherInterval))
                {
                    nonConflictingEntities.Add(entity);
                    continue;
                }

                if (newInterval.Covers(otherInterval))
                {
                    coveredEntities.Add(entity);
                    continue;
                }

                if (otherInterval.Item1 < newInterval.Item1)
                {
                    // Coming soon..
                    //var clone = entity.Clone();
                    //variantClone.End = dominantInterval.Item1;
                    //variants.Add(variantClone);
                }

            }
        }
    }

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

        internal static IEnumerable<T> RemoveAllButLatestVariants<T>(
            this IEnumerable<T> entities) where T : IObjectWithGuidID, IObjectWithValidTime
        {
            return entities
                .GroupBy(p => p.ID)
                .Select(g => g
                    .OrderBy(p => p.Start)
                    .LastOrDefault())
                .Where(p => p != null);
        }

        internal static IEnumerable<T> RemoveCurrentVariants<T>(
            this IEnumerable<T> entities,
            DateTime? historicalTime) where T : IObjectWithGuidID, IObjectWithValidTime
        {
            var time = historicalTime.HasValue
                ? historicalTime.Value
                : new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            return entities.Where(_ => _.End < time);
        }

    }
}
