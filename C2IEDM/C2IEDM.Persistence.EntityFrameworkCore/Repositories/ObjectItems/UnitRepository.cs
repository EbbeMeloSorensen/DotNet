using C2IEDM.Domain.Entities.ObjectItems.Organisations;
using C2IEDM.Persistence.Repositories.ObjectItems;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.ObjectItems;

public class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(Unit entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<Unit> entities)
    {
        throw new NotImplementedException();
    }
}