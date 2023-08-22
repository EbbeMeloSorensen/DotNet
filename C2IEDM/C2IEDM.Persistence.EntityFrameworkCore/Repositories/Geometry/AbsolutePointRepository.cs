using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class AbsolutePointRepository : Repository<AbsolutePoint>, IAbsolutePointRepository
{
    public AbsolutePointRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(AbsolutePoint entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<AbsolutePoint> entities)
    {
        throw new NotImplementedException();
    }
}