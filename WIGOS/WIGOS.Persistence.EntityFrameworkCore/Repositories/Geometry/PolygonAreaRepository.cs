using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class PolygonAreaRepository : Repository<PolygonArea>, IPolygonAreaRepository
    {
        public PolygonAreaRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(PolygonArea entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<PolygonArea> entities)
        {
            throw new NotImplementedException();
        }
    }
}