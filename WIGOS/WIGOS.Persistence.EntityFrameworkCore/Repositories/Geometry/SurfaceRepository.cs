using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class SurfaceRepository : Repository<Surface>, ISurfaceRepository
    {
        public SurfaceRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(Surface entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Surface> entities)
        {
            throw new NotImplementedException();
        }
    }
}