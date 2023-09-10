using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.ObjectItems;
using C2IEDM.Persistence.Repositories.ObjectItems;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.ObjectItems
{
    public class ObjectItemRepository : Repository<ObjectItem>, IObjectItemRepository
    {
        public ObjectItemRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(ObjectItem entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<ObjectItem> entities)
        {
            throw new NotImplementedException();
        }
    }
}
