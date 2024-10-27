using WIGOS.Domain.Entities.WIGOS.GeospatialLocations;
using WIGOS.Persistence.Repositories.WIGOS;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.WIGOS.GeospatialLocations
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

        public override Task UpdateRange(
            IEnumerable<Point> points)
        {
            throw new NotImplementedException();
        }
    }
}