using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Persistence.Repositories.Geometry;
using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class FanAreaRepository : Repository<FanArea>, IFanAreaRepository
{
    public FanAreaRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(FanArea entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<FanArea> entities)
    {
        throw new NotImplementedException();
    }
}