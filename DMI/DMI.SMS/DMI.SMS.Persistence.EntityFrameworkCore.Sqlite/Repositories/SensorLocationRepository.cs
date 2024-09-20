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

        public int GenerateUniqueObjectId()
        {
            var context = Context as SMSDbContext;

            if (context == null)
            {
                throw new InvalidCastException();
            }

            if (!context.SensorLocations.Any())
            {
                return 1;
            }

            return context.SensorLocations.Max(_ => _.ObjectId) + 1;
        }

        public string GenerateUniqueGlobalId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
