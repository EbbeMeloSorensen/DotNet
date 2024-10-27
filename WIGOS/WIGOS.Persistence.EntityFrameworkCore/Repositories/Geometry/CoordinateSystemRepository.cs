using WIGOS.Domain.Entities.Geometry.CoordinateSystems;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class CoordinateSystemRepository : Repository<CoordinateSystem>, ICoordinateSystemRepository
    {
        public CoordinateSystemRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(CoordinateSystem entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<CoordinateSystem> entities)
        {
            throw new NotImplementedException();
        }
    }
}