using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.Repositories.Geometry;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class EllipseRepository : Repository<Ellipse>, IEllipseRepository
    {
        public EllipseRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(Ellipse entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<Ellipse> entities)
        {
            throw new NotImplementedException();
        }
    }
}
