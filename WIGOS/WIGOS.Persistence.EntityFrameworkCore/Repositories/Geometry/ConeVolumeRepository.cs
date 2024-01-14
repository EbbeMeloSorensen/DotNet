using WIGOS.Domain.Entities.Geometry.Locations.GeometricVolumes;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class ConeVolumeRepository : Repository<ConeVolume>, IConeVolumeRepository
    {
        public ConeVolumeRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(ConeVolume entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<ConeVolume> entities)
        {
            throw new NotImplementedException();
        }
    }
}