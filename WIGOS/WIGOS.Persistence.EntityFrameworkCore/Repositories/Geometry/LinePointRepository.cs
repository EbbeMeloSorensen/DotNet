using WIGOS.Domain.Entities.Geometry.Locations.Line;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class LinePointRepository : Repository<LinePoint>, ILinePointRepository
    {
        public LinePointRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(LinePoint entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<LinePoint> entities)
        {
            throw new NotImplementedException();
        }
    }
}
