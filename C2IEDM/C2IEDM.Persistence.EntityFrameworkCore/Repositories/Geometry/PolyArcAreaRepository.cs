using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class PolyArcAreaRepository : Repository<PolyArcArea>, IPolyArcAreaRepository
{
    public PolyArcAreaRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(PolyArcArea entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<PolyArcArea> entities)
    {
        throw new NotImplementedException();
    }
}