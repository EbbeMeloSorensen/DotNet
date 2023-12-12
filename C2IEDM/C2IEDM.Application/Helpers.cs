using System.Linq.Expressions;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

namespace C2IEDM.Application
{
    public static class Helpers
    {
        public static Expression<Func<ObservingFacility, bool>> ObservationFacilityFilterAsExpression(
            DateTime? historicalTimeOfInterest,
            DateTime? databaseTimeOfInterest,
            string nameFilterInUppercase)
        {
            if (historicalTimeOfInterest.HasValue &&
                databaseTimeOfInterest.HasValue)
            {
                return _ =>
                    _.Created <= databaseTimeOfInterest.Value && // Kun rækker skrevet før pågældende tidspunkt
                    _.Superseded >
                    databaseTimeOfInterest
                        .Value && // Kun rækker, der er "gældende" (eller var gældende på pågældende tidspunkt)
                    _.DateEstablished <= historicalTimeOfInterest.Value && // ->
                    _.DateClosed >
                    historicalTimeOfInterest
                        .Value && // Kun rækker, hvis virkningstidsinterval skærer historical time of interest
                    _.Name.ToUpper().Contains(nameFilterInUppercase);
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

            return _ =>
                _.Superseded == DateTime.MaxValue && // Kun rækker, der er gældende
                _.DateClosed ==
                DateTime.MaxValue && // Kun rækker, hvis virkningstidsinterval skærer historical time of interest, dvs stationer, der er aktive i dag
                _.Name.ToUpper().Contains(nameFilterInUppercase);
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
