using WIGOS.Domain.Entities.Geometry.Locations.GeometricVolumes;
using WIGOS.Persistence.Repositories.Geometry;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WIGOS.Persistence.EntityFrameworkCore.Repositories.Geometry
{
    public class GeometricVolumeRepository : Repository<GeometricVolume>, IGeometricVolumeRepository
    {
        public GeometricVolumeRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(GeometricVolume entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<GeometricVolume> entities)
        {
            throw new NotImplementedException();
        }
    }
}