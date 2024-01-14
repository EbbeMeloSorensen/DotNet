using WIGOS.Domain.Entities.ObjectItems;
using WIGOS.Persistence.Repositories.ObjectItems;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.ObjectItems
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
