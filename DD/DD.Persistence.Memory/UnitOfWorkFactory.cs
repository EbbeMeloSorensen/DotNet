using System;
using System.Threading.Tasks;
using Craft.Logging;
using DD.Domain;
using DD.Persistence.Memory.Repositories;

namespace DD.Persistence.Memory
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private CreatureTypeRepository _creatureTypeRepository;
        private SceneRepository _sceneRepository;

        public UnitOfWorkFactory()
        {
            _creatureTypeRepository = new CreatureTypeRepository();
            _sceneRepository = new SceneRepository();

            Populate();
        }

        public void Initialize(
            ILogger logger)
        {
        }

        public Task<bool> CheckRepositoryConnection()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(
                _creatureTypeRepository,
                _sceneRepository);
        }

        private void Populate()
        {
            _creatureTypeRepository.Add(new CreatureType("Ettin", 30, 5, 11, 0, 10, null));
            _creatureTypeRepository.Add(new CreatureType("Lich", 30, 5, 11, 0, 10, null));
            _creatureTypeRepository.Add(new CreatureType("Dragon", 30, 5, 11, 0, 10, null));
            _creatureTypeRepository.Add(new CreatureType("Ghoul", 30, 5, 11, 0, 10, null));
            _creatureTypeRepository.Add(new CreatureType("Mind Flayer", 30, 5, 11, 0, 10, null));
            _creatureTypeRepository.Add(new CreatureType("Beholder", 30, 5, 11, 0, 10, null));
        }
    }
}
