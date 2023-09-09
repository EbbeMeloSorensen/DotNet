using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Persistence.Repositories.Geometry;
using C2IEDM.Domain.Entities.Geometry.Locations.Surfaces;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class CorridorAreaRepository : Repository<CorridorArea>, ICorridorAreaRepository
{
    public CorridorAreaRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(CorridorArea entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<CorridorArea> entities)
    {
        throw new NotImplementedException();
    }
}