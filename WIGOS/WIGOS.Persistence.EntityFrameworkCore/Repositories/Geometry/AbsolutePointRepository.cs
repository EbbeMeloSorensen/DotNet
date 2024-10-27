using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class AbsolutePointRepository : Repository<AbsolutePoint>, IAbsolutePointRepository
    {
        public AbsolutePointRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(AbsolutePoint entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<AbsolutePoint> entities)
        {
            throw new NotImplementedException();
        }
    }
}