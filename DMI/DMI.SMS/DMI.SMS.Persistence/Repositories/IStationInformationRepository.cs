using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.Repositories
{
    public interface IStationInformationRepository : IRepositoryType2<StationInformation>
    {
        StationInformation GetByGlobalId(
            string globalId);

        StationInformation Get(int id);

        StationInformation GetStationInformationWithContactPersons(int id);

        int GenerateUniqueObjectId();
        string GenerateUniqueGlobalId();
    }
}