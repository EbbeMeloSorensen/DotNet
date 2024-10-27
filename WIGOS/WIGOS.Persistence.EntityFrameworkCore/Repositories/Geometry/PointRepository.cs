using WIGOS.Domain.Entities.Geometry.Locations.Points;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class PointRepository : Repository<Point>, IPointRepository
    {
        public PointRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(Point entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Point> entities)
        {
            throw new NotImplementedException();
        }
    }
}