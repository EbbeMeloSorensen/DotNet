using Craft.Persistence;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.Repositories
{
    public interface IElevationAnglesRepository : IRepository<ElevationAngles>
    {
        int GenerateUniqueObjectId();
        string GenerateUniqueGlobalId();
    }
}