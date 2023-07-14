using System;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IObservingFacilityRepository ObservingFacilities { get; }
        ITimeSeriesRepository TimeSeries { get; }
        IObservationRepository Observations { get; }

        int Complete();
    }
}
