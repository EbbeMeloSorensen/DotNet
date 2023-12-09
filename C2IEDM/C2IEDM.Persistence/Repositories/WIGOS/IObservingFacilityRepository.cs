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
        Guid id);

    Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
        Expression<Func<ObservingFacility, bool>> predicate);

    Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
        IList<Expression<Func<ObservingFacility, bool>>> predicates);
}