using WIGOS.Domain.Entities.Geometry.Locations.Surfaces;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class TrackAreaRepository : Repository<TrackArea>, ITrackAreaRepository
    {
        public TrackAreaRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(TrackArea entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<TrackArea> entities)
        {
            throw new NotImplementedException();
        }
    }
}