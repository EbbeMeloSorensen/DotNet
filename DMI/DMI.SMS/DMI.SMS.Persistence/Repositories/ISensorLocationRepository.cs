using Craft.Persistence;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.Repositories
{
    public interface ISensorLocationRepository : IRepository<SensorLocation>
    {
        SensorLocation GetByGlobalId(
            string globalId);

        int GenerateUniqueObjectId();

        string GenerateUniqueGlobalId();
    }
}