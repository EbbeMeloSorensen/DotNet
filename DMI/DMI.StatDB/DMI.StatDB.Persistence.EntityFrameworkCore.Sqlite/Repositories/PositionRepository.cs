using Craft.Persistence.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class PositionRepository : Repository<Position>, IPositionRepository
    {
        public PositionRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(Position entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }
    }
}
