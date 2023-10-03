using Microsoft.EntityFrameworkCore;
using C2IEDM.Domain.Entities.WIGOS.GeospatialLocations;
using C2IEDM.Persistence.Repositories.WIGOS;
using Craft.Persistence.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.WIGOS.GeospatialLocations;

public class GeospatialLocationRepository : Repository<GeospatialLocation>, IGeospatialLocationRepository
{
    public GeospatialLocationRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        var context = Context as C2IEDMDbContextBase;

        context.RemoveRange(context.GeospatialLocations);
        context.SaveChanges();
    }

    public override void Update(GeospatialLocation entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<GeospatialLocation> entities)
    {
        throw new NotImplementedException();
    }
}
