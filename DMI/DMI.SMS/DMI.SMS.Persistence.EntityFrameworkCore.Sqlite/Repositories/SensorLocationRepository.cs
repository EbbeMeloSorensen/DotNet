using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.EntityFrameworkCore.Sqlite.Repositories
{
    public class SensorLocationRepository : Repository<SensorLocation>, ISensorLocationRepository
    {
        public SensorLocationRepository(DbContext context) : base(context)
        {
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(SensorLocation entity)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRange(IEnumerable<SensorLocation> entities)
        {
            throw new NotImplementedException();
        }

        public string GenerateUniqueGlobalId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
