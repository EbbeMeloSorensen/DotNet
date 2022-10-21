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
        private Dictionary<Guid, Record> _personCache;
        private Dictionary<Guid, RecordAssociation> _personAssociationCache;

        public override IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _personCache = new Dictionary<Guid, Record>();
            _personAssociationCache = new Dictionary<Guid, RecordAssociation>();
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

        public override void CreateRecord(
            Record person)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Records.Add(person);
                unitOfWork.Complete();
            }

            var cacheObj = person.Clone();
            _personCache[person.Id] = cacheObj;

            OnRecordCreated(cacheObj);
        }

        public override void CreateRecordAssociation(
            RecordAssociation personAssociation)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.RecordAssociations.Add(personAssociation);
                unitOfWork.Complete();
            }

            var modelObjForCache = personAssociation.Clone();

            var subjectPerson = GetRecord(personAssociation.SubjectRecordId);
            var objectPerson = GetRecord(personAssociation.ObjectRecordId);

            modelObjForCache.LinkToRecords(subjectPerson, objectPerson);
            _personAssociationCache[modelObjForCache.Id] = modelObjForCache;
        }

        public override int CountRecords(
            Expression<Func<Record, bool>> predicate)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.Records.Count(predicate);

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override Record GetRecord(
            Guid id)
        {
            if (_personCache.ContainsKey(id))
            {
                return _personCache[id];
            }

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var person = unitOfWork.Records.Get(id).Clone();
                _personCache[id] = person;
                return person;
            }
        }

        public override Record GetRecordWithAssociations(
            Guid id)
        {
            Record personFromRepository;

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                personFromRepository = unitOfWork.Records.GetRecordIncludingAssociations(id);
            }

            var person = IncludeInCache(personFromRepository);

            person.ObjectRecords = personFromRepository.ObjectRecords?.Select(IncludeInCache).ToList();
            person.SubjectRecords = personFromRepository.SubjectRecords?.Select(IncludeInCache).ToList();

            return person;
        }

        public override IList<Record> GetAllRecords()
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stationInformations = new List<Record>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.Records.GetAll().ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stationInformations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stationInformations;
        }

        public override IList<RecordAssociation> GetAllRecordAssociations()
        {
            IList<RecordAssociation> personAssociations;

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                personAssociations = unitOfWork.RecordAssociations.GetAll()
                    .Select(pa => pa.Clone())
                    .ToList();
            }

            return personAssociations;
        }

        public override IList<Record> FindRecords(
            Expression<Func<Record, bool>> predicate)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var people = new List<Record>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var peopleFromRepository = unitOfWork.Records.Find(predicate).ToList();

                peopleFromRepository.ForEach(p =>
                {
                    var cachePerson = IncludeInCache(p);
                    people.Add(cachePerson);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return people;
        }

        public override IList<Record> FindRecords(
            IList<Expression<Func<Record, bool>>> predicates)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stationInformations = new List<Record>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.Records.Find(predicates).ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stationInformations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stationInformations;
        }

        public override void UpdateRecord(
            Record record)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRecords(
            IList<Record> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Records.UpdateRange(people);
                unitOfWork.Complete();
            }

            // Update the people of the cache too
            foreach (var person in people)
            {
                var cacheObj = GetRecord(person.Id);
                cacheObj.CopyAttributes(person);
            }

            OnPeopleUpdated(people);
        }

        public override void UpdateRecordAssociation(
            RecordAssociation personAssociation)
        {
            throw new NotImplementedException();
        }

        public override void DeleteRecord(
            Record record)
        {
            throw new NotImplementedException();
        }

        public override void DeleteRecords(
            IList<Record> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = people.Select(p => p.Id).ToList();

                var peopleForDeletion = unitOfWork.Records
                    .GetRecordsIncludingAssociations(p => ids.Contains(p.Id))
                    .ToList();

                var personAssociationsForDeletion = peopleForDeletion
                    .SelectMany(p => p.ObjectRecords)
                    .Concat(peopleForDeletion.SelectMany(p => p.SubjectRecords))
                    .ToList();

                unitOfWork.RecordAssociations.RemoveRange(personAssociationsForDeletion);
                unitOfWork.Records.RemoveRange(peopleForDeletion);
                unitOfWork.Complete();

                personAssociationsForDeletion.ForEach(RemoveFromCache);
                peopleForDeletion.ForEach(RemoveFromCache);
            }

            OnPeopleDeleted(people);
        }

        public override void DeleteRecordAssociations(
            IList<RecordAssociation> personAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = personAssociations.Select(p => p.Id).ToList();
                var forDeletion = unitOfWork.RecordAssociations.Find(pa => ids.Contains(pa.Id));

                unitOfWork.RecordAssociations.RemoveRange(forDeletion);
                unitOfWork.Complete();
            }

            // Update memory objects
            personAssociations.ToList().ForEach(pa =>
            {
                pa.SubjectRecord?.ObjectRecords?.Remove(pa);
                pa.ObjectRecord?.SubjectRecords?.Remove(pa);
                _personAssociationCache.Remove(pa.Id);
            });
        }

        protected override void LoadRecords(
            IList<Record> people)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Records.AddRange(people);
                unitOfWork.Complete();
            }

            _personCache.Clear();
            // We don't update the cache, because it might be a lot of data
            // On the contrary, we clear the cache, so we're not looking at obsolete data
        }

        protected override void LoadRecordAssociations(
            IList<RecordAssociation> personAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.RecordAssociations.AddRange(personAssociations);
                unitOfWork.Complete();
            }

            _personAssociationCache.Clear();
        }

        private Record IncludeInCache(
            Record personFromRepository)
        {
            if (_personCache.ContainsKey(personFromRepository.Id))
            {
                return _personCache[personFromRepository.Id];
            }

            var person = personFromRepository.Clone();
            _personCache[person.Id] = person;

            return person;
        }

        private RecordAssociation IncludeInCache(
            RecordAssociation personAssociationFromRepository)
        {
            if (_personAssociationCache.ContainsKey(personAssociationFromRepository.Id))
            {
                return _personAssociationCache[personAssociationFromRepository.Id];
            }

            var personAssociation = personAssociationFromRepository.Clone();

            personAssociation.LinkToRecords(
                IncludeInCache(personAssociationFromRepository.SubjectRecord),
                IncludeInCache(personAssociationFromRepository.ObjectRecord));

            _personAssociationCache[personAssociation.Id] = personAssociation;

            return personAssociation;
        }

        private void RemoveFromCache(
            Record person)
        {
            if (!_personCache.ContainsKey(person.Id)) return;

            _personCache.Remove(person.Id);
        }

        private void RemoveFromCache(
            RecordAssociation personAssociation)
        {
            if (!_personAssociationCache.ContainsKey(personAssociation.Id)) return;

            _personAssociationCache[personAssociation.Id].DecoupleFromPeople();
            _personAssociationCache.Remove(personAssociation.Id);
        }
    }
}
