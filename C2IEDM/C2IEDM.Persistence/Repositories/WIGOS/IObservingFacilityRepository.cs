using System.Linq.Expressions;
using Craft.Persistence;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;

namespace C2IEDM.Persistence.Repositories.WIGOS;

public interface IObservingFacilityRepository : IRepository<ObservingFacility>
{
    Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
        IList<Expression<Func<ObservingFacility, bool>>> predicates);
}