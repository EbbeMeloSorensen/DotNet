using WIGOS.Domain.Entities.Geometry.Locations.GeometricVolumes;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class SphereVolumeRepository : Repository<SphereVolume>, ISphereVolumeRepository
    {
        public SphereVolumeRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(SphereVolume entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<SphereVolume> entities)
        {
            throw new NotImplementedException();
        }
    }
}