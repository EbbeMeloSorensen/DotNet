using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DD.Application;
using DD.Domain;
using DD.IO;
using DD.Persistence;

namespace DD.UIDataProvider.Persistence
{
    public class UIDataProvider : UIDataProviderBase
    {
        private Dictionary<int, CreatureType> _creatureTypeCache;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _creatureTypeCache = new Dictionary<int, CreatureType>();
        }

        public UIDataProvider(IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
        }

        public override async Task<bool> CheckConnection()
        {
            return await UnitOfWorkFactory.CheckRepositoryConnection();
        }

        public override void CreateCreatureType(
            CreatureType creatureType)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.CreatureTypes.Add(creatureType);
                unitOfWork.Complete();
            }

            var cacheObj = creatureType.Clone();
            _creatureTypeCache[creatureType.Id] = cacheObj;

            OnCreatureTypeCreated(cacheObj);
        }

        public override CreatureType GetCreatureType(int id)
        {
            throw new NotImplementedException();
        }

        public override IList<CreatureType> GetAllCreatureTypes()
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var creatureTypes = new List<CreatureType>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var creatureTypesFromRepository = unitOfWork.CreatureTypes.GetAll().ToList();

                creatureTypesFromRepository.ForEach(s =>
                {
                    var cacheCreatureType = IncludeInCache(s);
                    creatureTypes.Add(cacheCreatureType);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return creatureTypes;
        }

        private CreatureType IncludeInCache(
            CreatureType creatureTypeFromRepository)
        {
            if (_creatureTypeCache.ContainsKey(creatureTypeFromRepository.Id))
            {
                return _creatureTypeCache[creatureTypeFromRepository.Id];
            }

            var creatureType = creatureTypeFromRepository.Clone();
            _creatureTypeCache[creatureType.Id] = creatureType;

            return creatureType;
        }
    }
}
