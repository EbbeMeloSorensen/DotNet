using System;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IObservationRepository Observations { get; }

        int Complete();
    }
}
