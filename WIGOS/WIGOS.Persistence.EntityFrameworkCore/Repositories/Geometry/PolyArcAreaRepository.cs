using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class PolyArcAreaRepository : Repository<PolyArcArea>, IPolyArcAreaRepository
    {
        public PolyArcAreaRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(PolyArcArea entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<PolyArcArea> entities)
        {
            throw new NotImplementedException();
        }
    }
}