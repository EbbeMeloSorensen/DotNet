using System;
using DD.Persistence.Repositories;

namespace DD.Persistence.Memory
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICreatureTypeRepository CreatureTypes { get; }
        public ISceneRepository Scenes { get; }

        public UnitOfWork(
            ICreatureTypeRepository creatureTypeRepository,
            ISceneRepository sceneRepository)
        {
            CreatureTypes = creatureTypeRepository;
            Scenes = sceneRepository;
        }

        public void Dispose()
        {
        }

        public int Complete()
        {
            return 0;
        }
    }
}
