using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class CorridorAreaRepository : Repository<CorridorArea>, ICorridorAreaRepository
    {
        public CorridorAreaRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(CorridorArea entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<CorridorArea> entities)
        {
            throw new NotImplementedException();
        }
    }
}