using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain;
using PR.Domain.Entities;
using PR.Persistence;
using PR.IO;
using PR.Application;

namespace PR.UIDataProvider.Persistence
{
    public class UIDataProvider : UIDataProviderBase
    {
        private Dictionary<Guid, Person> _personCache;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _personCache = new Dictionary<Guid, Person>();
        }

        public override void Initialize(
            ILogger logger)
        {
            base.Initialize(logger);

            UnitOfWorkFactory.Initialize(logger);
        }

        public override async Task<bool> CheckConnection()
        {
            return await UnitOfWorkFactory.CheckRepositoryConnection();
        }

        public override void CreatePerson(Person person)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.Add(person);
                unitOfWork.Complete();
            }

            var cacheObj = person.Clone();
            _personCache[person.Id] = cacheObj;
        }

        public override Person GetPerson(Guid id)
        {
            if (_personCache.ContainsKey(id))
            {
                return _personCache[id];
            }

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var person = unitOfWork.People.Get(id).Clone();
                _personCache[id] = person;
                return person;
            }
        }
    }
}
