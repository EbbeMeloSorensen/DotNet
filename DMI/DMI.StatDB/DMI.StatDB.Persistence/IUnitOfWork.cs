using System;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IStationRepository Stations { get; }
        IPositionRepository Positions { get; }

        int Complete();
    }
}
