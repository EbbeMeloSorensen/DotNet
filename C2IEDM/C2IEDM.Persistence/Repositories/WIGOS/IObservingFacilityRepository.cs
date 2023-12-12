using System.Linq.Expressions;
using Craft.Persistence;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

namespace C2IEDM.Persistence.Repositories.WIGOS;

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