using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class FanAreaRepository : Repository<FanArea>, IFanAreaRepository
    {
        public FanAreaRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(FanArea entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<FanArea> entities)
        {
            throw new NotImplementedException();
        }
    }
}