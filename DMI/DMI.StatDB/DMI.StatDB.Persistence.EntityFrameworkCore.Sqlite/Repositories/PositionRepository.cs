using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class PositionRepository : Repository<Position>, IPositionRepository
    {
        public PositionRepository(DbContext context) : base(context)
        {
        }

        public override Task Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(
            Position entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(
            IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }
    }
}
