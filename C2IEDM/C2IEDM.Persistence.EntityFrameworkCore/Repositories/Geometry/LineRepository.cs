using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Persistence.Repositories.Geometry;
using C2IEDM.Domain.Entities.Geometry.Locations;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class LineRepository : Repository<Line>, ILineRepository
    {
        public LineRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(Line entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<Line> entities)
        {
            throw new NotImplementedException();
        }
    }
}
