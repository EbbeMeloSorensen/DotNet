using Craft.Persistence;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.Repositories
{
    public interface IObservingFacilityRepository : IRepository<ObservingFacility>
    {
        ObservingFacility Get(
            int id);

        ObservingFacility GetIncludingTimeSeries(
            int id);
    }
}
