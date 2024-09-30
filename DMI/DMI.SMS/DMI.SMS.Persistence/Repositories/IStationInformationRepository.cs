using Craft.Persistence;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.Repositories
{
    public interface IStationInformationRepository : IRepository<StationInformation>
    {
        StationInformation GetByGlobalId(
            string globalId);

        StationInformation Get(int id);

        StationInformation GetStationInformationWithContactPersons(int id);

        int GenerateUniqueObjectId();
        string GenerateUniqueGlobalId();
    }
}