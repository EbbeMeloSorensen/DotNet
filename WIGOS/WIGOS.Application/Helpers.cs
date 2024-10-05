using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using Craft.Persistence;
using System.Linq.Expressions;

namespace WIGOS.Application
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

            if (!string.IsNullOrEmpty(nameFilterInUppercase))
            {
                predicates.Add(_ => _.Name.ToUpper().Contains(nameFilterInUppercase));
            }

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
