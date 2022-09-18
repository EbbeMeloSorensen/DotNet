using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public override IUnitOfWorkFactory UnitOfWorkFactory { get; }

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

        public override IList<Person> GetAllPeople()
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stationInformations = new List<Person>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.People.GetAll().ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stationInformations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stationInformations;
        }

        public override IList<Person> FindPeople(
            Expression<Func<Person, bool>> predicate)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var people = new List<Person>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var peopleFromRepository = unitOfWork.People.Find(predicate).ToList();

                peopleFromRepository.ForEach(p =>
                {
                    var cachePerson = IncludeInCache(p);
                    people.Add(cachePerson);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return people;
        }

        public override IList<Person> FindPeople(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stationInformations = new List<Person>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.People.Find(predicates).ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stationInformations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stationInformations;
        }

        protected override void LoadPeople(
            IList<Person> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.AddRange(people);
                unitOfWork.Complete();
            }

            _personCache.Clear();
            // We don't update the cache, because it might be a lot of data
            // On the contrary, we clear the cache, so we're not looking at obsolete data
        }

        private Person IncludeInCache(
            Person personFromRepository)
        {
            if (_personCache.ContainsKey(personFromRepository.Id))
            {
                return _personCache[personFromRepository.Id];
            }

            var person = personFromRepository.Clone();
            _personCache[person.Id] = person;

            return person;
        }

        private void RemoveFromCache(Person person)
        {
            if (!_personCache.ContainsKey(person.Id)) return;

            _personCache.Remove(person.Id);
        }
    }
}
