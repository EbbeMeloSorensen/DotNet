using WIGOS.Domain.Entities.Geometry;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class VerticalDistanceRepository : Repository<VerticalDistance>, IVerticalDistanceRepository
    {
        public VerticalDistanceRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(VerticalDistance entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<VerticalDistance> entities)
        {
            throw new NotImplementedException();
        }
    }
}