using C2IEDM.Domain.Entities.Geometry.Locations.GeometricVolumes;
using C2IEDM.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C2IEDM.Persistence.EntityFrameworkCore.Repositories.Geometry;

public class SphereVolumeRepository : Repository<SphereVolume>, ISphereVolumeRepository
{
    public SphereVolumeRepository(DbContext context) : base(context)
    {
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override void Update(SphereVolume entity)
    {
        throw new NotImplementedException();
    }

    public override void UpdateRange(IEnumerable<SphereVolume> entities)
    {
        throw new NotImplementedException();
    }
}