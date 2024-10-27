using WIGOS.Domain.Entities.ObjectItems.Organisations;
using WIGOS.Persistence.Repositories.ObjectItems;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.ObjectItems
{
    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(Unit entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Unit> entities)
        {
            throw new NotImplementedException();
        }
    }
}