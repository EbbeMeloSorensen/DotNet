using WIGOS.Domain.Entities.Geometry.CoordinateSystems;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class PointReferenceRepository : Repository<PointReference>, IPointReferenceRepository
    {
        public PointReferenceRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(PointReference entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<PointReference> entities)
        {
            throw new NotImplementedException();
        }
    }
}