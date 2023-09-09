using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using C2IEDM.Persistence.Repositories.Geometry;
using C2IEDM.Domain.Entities.Geometry.Locations;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry
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
