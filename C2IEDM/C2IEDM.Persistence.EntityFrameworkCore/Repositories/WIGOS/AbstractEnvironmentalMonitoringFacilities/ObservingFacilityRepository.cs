using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Craft.Persistence;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using C2IEDM.Persistence.Repositories.WIGOS;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.WIGOS.AbstractEnvironmentalMonitoringFacilities;

public class ObservingFacilityRepository : Repository<ObservingFacility>, IObservingFacilityRepository
{
    private C2IEDMDbContextBase DbContext => Context as C2IEDMDbContextBase;

    public ObservingFacilityRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
        Expression<Func<ObservingFacility, bool>> predicate)
    {
        var predicates = new List<Expression<Func<ObservingFacility, bool>>>
        {
            predicate
        };

        return FindIncludingGeospatialLocations(predicates);
    }

    public Dictionary<ObservingFacility, List<GeospatialLocation>> FindIncludingGeospatialLocations(
        IList<Expression<Func<ObservingFacility, bool>>> predicates)
    {
        var predicate = predicates.Aggregate((c, n) => c.And(n));

        var observingFacilities = DbContext.ObservingFacilities
            .Where(predicate)
            .ToList();

        var observingFacilityObjectIds = observingFacilities
            .Select(_ => _.ObjectId)
            .ToList();

        var geospatialLocations = DbContext.GeospatialLocations
            .Where(_ => 
                _.Superseded == DateTime.MaxValue && 
                observingFacilityObjectIds.Contains(_.AbstractEnvironmentalMonitoringFacilityObjectId))
            .ToList();

        var geospatialLocationGroups = geospatialLocations.
            GroupBy(_ => _.AbstractEnvironmentalMonitoringFacilityObjectId);

        var result = observingFacilities.ToDictionary(
            of => of,
            of =>
            {
                return geospatialLocationGroups
                    .Single(glg => glg.Key == of.ObjectId)
                    .ToList();
            });

        return result;
    }

    public override void Update(ObservingFacility entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(
        IEnumerable<ObservingFacility> observingFacilities)
    {
        var updatedObservingFacilities= observingFacilities.ToList();
        var ids = updatedObservingFacilities.Select(p => p.Id);
        var observingFacilitiesFromRepository = Find(p => ids.Contains(p.Id)).ToList();

        observingFacilitiesFromRepository.ForEach(ofRepo =>
        {
            var updatedObservingFacility = updatedObservingFacilities.Single(ofUpd => ofUpd.Id == ofRepo.Id);

            ofRepo.Name = updatedObservingFacility.Name;
            ofRepo.DateEstablished = updatedObservingFacility.DateEstablished;
            ofRepo.DateClosed = updatedObservingFacility.DateClosed;
        });
    }
}
