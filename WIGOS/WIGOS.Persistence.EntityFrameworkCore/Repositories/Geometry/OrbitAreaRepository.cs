using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class OrbitAreaRepository : Repository<OrbitArea>, IOrbitAreaRepository
    {
        public OrbitAreaRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(OrbitArea entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<OrbitArea> entities)
        {
            throw new NotImplementedException();
        }
    }
}