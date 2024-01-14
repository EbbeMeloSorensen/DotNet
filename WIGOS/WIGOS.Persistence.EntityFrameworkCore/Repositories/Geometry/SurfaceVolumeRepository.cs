using WIGOS.Domain.Entities.Geometry.Locations.GeometricVolumes;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
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
}