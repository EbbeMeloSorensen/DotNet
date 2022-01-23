using System;
using DD.Persistence.Repositories;

namespace DD.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ICreatureTypeRepository CreatureTypes { get; }
        ISceneRepository Scenes { get; }

        int Complete();
    }
}
