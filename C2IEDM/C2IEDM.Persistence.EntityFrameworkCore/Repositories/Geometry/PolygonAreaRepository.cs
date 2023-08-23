using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.Repositories.Geometry;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class PolygonAreaRepository : Repository<PolygonArea>, IPolygonAreaRepository
{
    public PolygonAreaRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(PolygonArea entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<PolygonArea> entities)
    {
        throw new NotImplementedException();
    }
}