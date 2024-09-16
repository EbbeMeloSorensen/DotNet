using Craft.Persistence;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.Repositories
{
    public interface ISensorLocationRepository : IRepository<SensorLocation>
    {
        int GenerateUniqueObjectId();
        string GenerateUniqueGlobalId();
    }
}