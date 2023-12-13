using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using C2IEDM.Persistence.Repositories.WIGOS;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.WIGOS.GeospatialLocations;

public class GeospatialLocationRepository : Repository<GeospatialLocation>, IGeospatialLocationRepository
{
    private C2IEDMDbContextBase DbContext => Context as C2IEDMDbContextBase;

    public GeospatialLocationRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        var context = Context as C2IEDMDbContextBase;

        context.RemoveRange(context.GeospatialLocations);
        context.SaveChanges();
    }

    public GeospatialLocation Get(
        Guid id)
    {
        return DbContext.GeospatialLocations.SingleOrDefault(_ => _.Id == id) ?? throw new InvalidOperationException();
    }

    public override void Update(
        GeospatialLocation geospatialLocation)
    {
        var geospatialLocationFromRepository = Get(geospatialLocation.Id);

        // I praksis er det kun superseded, vi har lov til at ændre, men sørg lige for at metoden virker generelt
        geospatialLocationFromRepository.Superseded = geospatialLocation.Superseded;
    }

    public override void UpdateRange(
        IEnumerable<GeospatialLocation> geospatialLocations)
    {
        var updatedGeospatialLocations = geospatialLocations.ToList();
        var ids = updatedGeospatialLocations.Select(p => p.Id);
        var geospatialLocationsFromRepository = Find(p => ids.Contains(p.Id)).ToList();

        geospatialLocationsFromRepository.ForEach(glRepo =>
        {
            var updatedGeospatialLocation = updatedGeospatialLocations.Single(glUpd => glUpd.Id == glRepo.Id);

            // I praksis ændrer vi kun den her
            glRepo.Superseded = updatedGeospatialLocation.Superseded;
        });
    }
}
