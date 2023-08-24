using C2IEDM.Domain.Entities.Geometry;
using C2IEDM.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class SurfaceVolumeRepository : Repository<SurfaceVolume>, ISurfaceVolumeRepository
{
    public SurfaceVolumeRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(SurfaceVolume entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<SurfaceVolume> entities)
    {
        throw new NotImplementedException();
    }
}