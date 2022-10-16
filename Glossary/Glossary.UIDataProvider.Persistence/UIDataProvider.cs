using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using Glossary.Domain;
using Glossary.Domain.Entities;
using Glossary.Persistence;
using Glossary.IO;
using Glossary.Application;

namespace Glossary.UIDataProvider.Persistence
{
    public class UIDataProvider : UIDataProviderBase
    {
        private Dictionary<Guid, Person> _personCache;
        private Dictionary<Guid, PersonAssociation> _personAssociationCache;

        public override IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _personCache = new Dictionary<Guid, Person>();
            _personAssociationCache = new Dictionary<Guid, PersonAssociation>();
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

        public override void CreatePerson(
            Person person)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.Add(person);
                unitOfWork.Complete();
            }

            var cacheObj = person.Clone();
            _personCache[person.Id] = cacheObj;

            OnPersonCreated(cacheObj);
        }

        public override void CreatePersonAssociation(
            PersonAssociation personAssociation)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.PersonAssociations.Add(personAssociation);
                unitOfWork.Complete();
            }

            var modelObjForCache = personAssociation.Clone();

            var subjectPerson = GetPerson(personAssociation.SubjectPersonId);
            var objectPerson = GetPerson(personAssociation.ObjectPersonId);

            modelObjForCache.LinkToPeople(subjectPerson, objectPerson);
            _personAssociationCache[modelObjForCache.Id] = modelObjForCache;
        }

        public override int CountPeople(
            Expression<Func<Person, bool>> predicate)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.People.Count(predicate);

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override Person GetPerson(
            Guid id)
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

        public override Person GetPersonWithAssociations(
            Guid id)
        {
            Person personFromRepository;

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                personFromRepository = unitOfWork.People.GetPersonIncludingAssociations(id);
            }

            var person = IncludeInCache(personFromRepository);

            person.ObjectPeople = personFromRepository.ObjectPeople?.Select(IncludeInCache).ToList();
            person.SubjectPeople = personFromRepository.SubjectPeople?.Select(IncludeInCache).ToList();

            return person;
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

        public override IList<PersonAssociation> GetAllPersonAssociations()
        {
            IList<PersonAssociation> personAssociations;

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                personAssociations = unitOfWork.PersonAssociations.GetAll()
                    .Select(pa => pa.Clone())
                    .ToList();
            }

            return personAssociations;
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

        public override void UpdatePerson(
            Person person)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePeople(
            IList<Person> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.People.UpdateRange(people);
                unitOfWork.Complete();
            }

            // Update the people of the cache too
            foreach (var person in people)
            {
                var cacheObj = GetPerson(person.Id);
                cacheObj.CopyAttributes(person);
            }

            OnPeopleUpdated(people);
        }

        public override void UpdatePersonAssociation(
            PersonAssociation personAssociation)
        {
            throw new NotImplementedException();
        }

        public override void DeletePerson(
            Person person)
        {
            throw new NotImplementedException();
        }

        public override void DeletePeople(
            IList<Person> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = people.Select(p => p.Id).ToList();

                var peopleForDeletion = unitOfWork.People
                    .GetPeopleIncludingAssociations(p => ids.Contains(p.Id))
                    .ToList();

                var personAssociationsForDeletion = peopleForDeletion
                    .SelectMany(p => p.ObjectPeople)
                    .Concat(peopleForDeletion.SelectMany(p => p.SubjectPeople))
                    .ToList();

                unitOfWork.PersonAssociations.RemoveRange(personAssociationsForDeletion);
                unitOfWork.People.RemoveRange(peopleForDeletion);
                unitOfWork.Complete();

                personAssociationsForDeletion.ForEach(RemoveFromCache);
                peopleForDeletion.ForEach(RemoveFromCache);
            }

            OnPeopleDeleted(people);
        }

        public override void DeletePersonAssociations(
            IList<PersonAssociation> personAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = personAssociations.Select(p => p.Id).ToList();
                var forDeletion = unitOfWork.PersonAssociations.Find(pa => ids.Contains(pa.Id));

                unitOfWork.PersonAssociations.RemoveRange(forDeletion);
                unitOfWork.Complete();
            }

            // Update memory objects
            personAssociations.ToList().ForEach(pa =>
            {
                pa.SubjectPerson?.ObjectPeople?.Remove(pa);
                pa.ObjectPerson?.SubjectPeople?.Remove(pa);
                _personAssociationCache.Remove(pa.Id);
            });
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

        protected override void LoadPersonAssociations(
            IList<PersonAssociation> personAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.PersonAssociations.AddRange(personAssociations);
                unitOfWork.Complete();
            }

            _personAssociationCache.Clear();
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

        private PersonAssociation IncludeInCache(
            PersonAssociation personAssociationFromRepository)
        {
            if (_personAssociationCache.ContainsKey(personAssociationFromRepository.Id))
            {
                return _personAssociationCache[personAssociationFromRepository.Id];
            }

            var personAssociation = personAssociationFromRepository.Clone();

            personAssociation.LinkToPeople(
                IncludeInCache(personAssociationFromRepository.SubjectPerson),
                IncludeInCache(personAssociationFromRepository.ObjectPerson));

            _personAssociationCache[personAssociation.Id] = personAssociation;

            return personAssociation;
        }

        private void RemoveFromCache(
            Person person)
        {
            if (!_personCache.ContainsKey(person.Id)) return;

            _personCache.Remove(person.Id);
        }

        private void RemoveFromCache(
            PersonAssociation personAssociation)
        {
            if (!_personAssociationCache.ContainsKey(personAssociation.Id)) return;

            _personAssociationCache[personAssociation.Id].DecoupleFromPeople();
            _personAssociationCache.Remove(personAssociation.Id);
        }
    }
}
