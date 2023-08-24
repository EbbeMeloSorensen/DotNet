using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class TrackAreaRepository : Repository<TrackArea>, ITrackAreaRepository
{
    public TrackAreaRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(TrackArea entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<TrackArea> entities)
    {
        throw new NotImplementedException();
    }
}