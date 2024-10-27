using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
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

        public override Task Update(Ellipse entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Ellipse> entities)
        {
            throw new NotImplementedException();
        }
    }
}
