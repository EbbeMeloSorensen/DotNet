using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using Craft.Persistence;
using System.Linq.Expressions;

namespace WIGOS.Persistence.Repositories.WIGOS
{
    public interface IObservingFacilityRepository : IRepository<ObservingFacility>
    {
        ObservingFacility Get(
            Guid id);

        Tuple<ObservingFacility, List<GeospatialLocation>> GetIncludingGeospatialLocations(
            Guid id,
            IList<Expression<Func<GeospatialLocation, bool>>> geospatialLocationPredicates);

        Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
            Expression<Func<ObservingFacility, bool>> observingFacilityPredicate,
            Expression<Func<GeospatialLocation, bool>> geospatialLocationPredicate);

        Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
            IList<Expression<Func<ObservingFacility, bool>>> observingFacilityPredicates,
            IList<Expression<Func<GeospatialLocation, bool>>> geospatialLocationPredicates);
    }
}