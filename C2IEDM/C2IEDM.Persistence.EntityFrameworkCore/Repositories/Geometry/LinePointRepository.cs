using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.Repositories.Geometry;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry
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
