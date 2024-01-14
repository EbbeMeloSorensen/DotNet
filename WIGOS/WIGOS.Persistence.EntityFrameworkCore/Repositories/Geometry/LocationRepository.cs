using WIGOS.Domain.Entities.Geometry.Locations;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        public LocationRepository(
            DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(Location entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<Location> entities)
        {
            throw new NotImplementedException();
        }
    }
}
