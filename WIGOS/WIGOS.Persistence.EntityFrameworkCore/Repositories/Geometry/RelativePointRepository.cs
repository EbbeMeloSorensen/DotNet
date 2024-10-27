using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class RelativePointRepository : Repository<RelativePoint>, IRelativePointRepository
    {
        public RelativePointRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(RelativePoint entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<RelativePoint> entities)
        {
            throw new NotImplementedException();
        }
    }
}