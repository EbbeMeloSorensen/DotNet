using System.Linq.Expressions;
using Craft.Persistence;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

namespace C2IEDM.Application
{
    public static class Helpers
    {
        public static Expression<Func<ObservingFacility, bool>> ObservationFacilityFilterAsExpression(
            DateTime? historicalTimeOfInterest,
            DateTime? databaseTimeOfInterest,
            bool includeActiveObservingFacilities,
            bool includeClosedObservingFacilities,
            string nameFilterInUppercase)
        {
            var predicates = new List<Expression<Func<ObservingFacility, bool>>>();

            if (databaseTimeOfInterest.HasValue)
            {
                predicates.Add(_ => _.Created <= databaseTimeOfInterest.Value);   // Kun rækker skrevet før pågældende tidspunkt
                predicates.Add(_ => _.Superseded > databaseTimeOfInterest.Value); // Kun rækker, der er "gældende" (eller var gældende på pågældende tidspunkt)
            }
            else
            {
                predicates.Add(_ => _.Superseded == DateTime.MaxValue);           // Kun rækker, der er "gældende" nu 
            }

            if (historicalTimeOfInterest.HasValue)
            {
                predicates.Add(_ => _.DateEstablished <= historicalTimeOfInterest.Value); // Kun observing facilities, der blev etableret før pågældende tidspunkt
            }

            if (includeActiveObservingFacilities && !includeClosedObservingFacilities)
            {
                var timeOfInterest = historicalTimeOfInterest.HasValue
                    ? historicalTimeOfInterest.Value
                    : DateTime.UtcNow;

                predicates.Add(_ => _.DateClosed > timeOfInterest);
            }
            else if (includeClosedObservingFacilities && !includeActiveObservingFacilities)
            {
                var timeOfInterest = historicalTimeOfInterest.HasValue
                    ? historicalTimeOfInterest.Value
                    : DateTime.UtcNow;

                predicates.Add(_ => _.DateClosed < timeOfInterest);
            }

            if (historicalTimeOfInterest.HasValue)
            {
                predicates.Add(_ => _.DateClosed > historicalTimeOfInterest.Value);
            }

            if (!string.IsNullOrEmpty(nameFilterInUppercase))
            {
                predicates.Add(_ => _.Name.ToUpper().Contains(nameFilterInUppercase));
            }

            return predicates.Aggregate((c, n) => c.And(n));

            // Old
            if (historicalTimeOfInterest.HasValue &&
                databaseTimeOfInterest.HasValue)
            {
                predicates.Add(_ => _.Created <= databaseTimeOfInterest.Value);   // Kun rækker skrevet før pågældende tidspunkt
                predicates.Add(_ => _.Superseded > databaseTimeOfInterest.Value); // Kun rækker, der er "gældende" (eller var gældende på pågældende tidspunkt)
                predicates.Add(_ => _.DateEstablished <= historicalTimeOfInterest.Value);
                predicates.Add(_ => _.DateClosed > historicalTimeOfInterest.Value);
                predicates.Add(_ => _.Name.ToUpper().Contains(nameFilterInUppercase));

                return predicates.Aggregate((c, n) => c.And(n));
            }

            if (databaseTimeOfInterest.HasValue)
            {
                return _ =>
                    _.Created <= databaseTimeOfInterest.Value && // Kun rækker skrevet før pågældende tidspunkt
                    _.Superseded >
                    databaseTimeOfInterest
                        .Value && // Kun rækker, der er "gældende" (eller var gældende på pågældende tidspunkt)
                    _.DateClosed >
                    databaseTimeOfInterest
                        .Value && // Kun rækker, hvis virkningstidsinterval skærer database time of interest, dvs stationer, der var aktive pågældende tidspunkt
                    _.Name.ToUpper().Contains(nameFilterInUppercase);
            }

            if (historicalTimeOfInterest.HasValue)
            {
                return _ =>
                    _.Superseded == DateTime.MaxValue && // Kun rækker, der er gældende
                    _.DateEstablished <= historicalTimeOfInterest.Value && // ->
                    _.DateClosed >
                    historicalTimeOfInterest
                        .Value && // Kun rækker, hvis virkningstidsinterval skærer historical time of interest
                    _.Name.ToUpper().Contains(nameFilterInUppercase);
            }

            predicates.Add(_ => _.Superseded == DateTime.MaxValue); // Kun rækker, der er gældende
            predicates.Add(_ => _.DateClosed == DateTime.MaxValue);
            predicates.Add(_ => _.Name.ToUpper().Contains(nameFilterInUppercase));

            return predicates.Aggregate((c, n) => c.And(n));
        }

        public static Expression<Func<GeospatialLocation, bool>> GeospatialLocationFilterAsExpression(
            DateTime? databaseTimeOfInterest)
        {
            if (databaseTimeOfInterest.HasValue)
            {
                return _ =>
                    _.Created <= databaseTimeOfInterest.Value && // Kun rækker skrevet før pågældende tidspunkt
                    _.Superseded > databaseTimeOfInterest.Value; // Kun rækker, der er "gældende" (eller var gældende på pågældende tidspunkt)
            }

            return _ =>
                _.Superseded == DateTime.MaxValue; // Kun rækker, der er gældende
        }
    }
}
