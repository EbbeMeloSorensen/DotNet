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
        private Dictionary<Guid, Record> _recordCache;
        private Dictionary<Guid, RecordAssociation> _recordAssociationCache;

        public override IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _recordCache = new Dictionary<Guid, Record>();
            _recordAssociationCache = new Dictionary<Guid, RecordAssociation>();
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
            Record record)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Records.Add(record);
                unitOfWork.Complete();
            }

            var cacheObj = record.Clone();
            _recordCache[record.Id] = cacheObj;

            OnRecordCreated(cacheObj);
        }

        public override void CreateRecordAssociation(
            RecordAssociation recordAssociation)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.RecordAssociations.Add(recordAssociation);
                unitOfWork.Complete();
            }

            var modelObjForCache = recordAssociation.Clone();

            var subjectRecord = GetRecord(recordAssociation.SubjectRecordId);
            var objectRecord = GetRecord(recordAssociation.ObjectRecordId);

            modelObjForCache.LinkToRecords(subjectRecord, objectRecord);
            _recordAssociationCache[modelObjForCache.Id] = modelObjForCache;
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
            if (_recordCache.ContainsKey(id))
            {
                return _recordCache[id];
            }

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var record = unitOfWork.Records.Get(id).Clone();
                _recordCache[id] = record;
                return record;
            }
        }

        public override Record GetRecordWithAssociations(
            Guid id)
        {
            Record recordFromRepository;

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                recordFromRepository = unitOfWork.Records.GetRecordIncludingAssociations(id);
            }

            var record = IncludeInCache(recordFromRepository);

            record.ObjectRecords = recordFromRepository.ObjectRecords?.Select(IncludeInCache).ToList();
            record.SubjectRecords = recordFromRepository.SubjectRecords?.Select(IncludeInCache).ToList();

            return record;
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
            IList<RecordAssociation> recordAssociations;

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                recordAssociations = unitOfWork.RecordAssociations.GetAll()
                    .Select(pa => pa.Clone())
                    .ToList();
            }

            return recordAssociations;
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
                    var cacheRecord = IncludeInCache(p);
                    people.Add(cacheRecord);
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
            IList<Record> records)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Records.UpdateRange(records);
                unitOfWork.Complete();
            }

            // Update the people of the cache too
            foreach (var record in records)
            {
                var cacheObj = GetRecord(record.Id);
                cacheObj.CopyAttributes(record);
            }

            OnPeopleUpdated(records);
        }

        public override void UpdateRecordAssociation(
            RecordAssociation recordAssociation)
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

                var recordAssociationsForDeletion = peopleForDeletion
                    .SelectMany(p => p.ObjectRecords)
                    .Concat(peopleForDeletion.SelectMany(p => p.SubjectRecords))
                    .ToList();

                unitOfWork.RecordAssociations.RemoveRange(recordAssociationsForDeletion);
                unitOfWork.Records.RemoveRange(peopleForDeletion);
                unitOfWork.Complete();

                recordAssociationsForDeletion.ForEach(RemoveFromCache);
                peopleForDeletion.ForEach(RemoveFromCache);
            }

            OnPeopleDeleted(people);
        }

        public override void DeleteRecordAssociations(
            IList<RecordAssociation> recordAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = recordAssociations.Select(p => p.Id).ToList();
                var forDeletion = unitOfWork.RecordAssociations.Find(pa => ids.Contains(pa.Id));

                unitOfWork.RecordAssociations.RemoveRange(forDeletion);
                unitOfWork.Complete();
            }

            // Update memory objects
            recordAssociations.ToList().ForEach(pa =>
            {
                pa.SubjectRecord?.ObjectRecords?.Remove(pa);
                pa.ObjectRecord?.SubjectRecords?.Remove(pa);
                _recordAssociationCache.Remove(pa.Id);
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

            _recordCache.Clear();
            // We don't update the cache, because it might be a lot of data
            // On the contrary, we clear the cache, so we're not looking at obsolete data
        }

        protected override void LoadRecordAssociations(
            IList<RecordAssociation> recordAssociations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.RecordAssociations.AddRange(recordAssociations);
                unitOfWork.Complete();
            }

            _recordAssociationCache.Clear();
        }

        private Record IncludeInCache(
            Record recordFromRepository)
        {
            if (_recordCache.ContainsKey(recordFromRepository.Id))
            {
                return _recordCache[recordFromRepository.Id];
            }

            var record = recordFromRepository.Clone();
            _recordCache[record.Id] = record;

            return record;
        }

        private RecordAssociation IncludeInCache(
            RecordAssociation recordAssociationFromRepository)
        {
            if (_recordAssociationCache.ContainsKey(recordAssociationFromRepository.Id))
            {
                return _recordAssociationCache[recordAssociationFromRepository.Id];
            }

            var recordAssociation = recordAssociationFromRepository.Clone();

            recordAssociation.LinkToRecords(
                IncludeInCache(recordAssociationFromRepository.SubjectRecord),
                IncludeInCache(recordAssociationFromRepository.ObjectRecord));

            _recordAssociationCache[recordAssociation.Id] = recordAssociation;

            return recordAssociation;
        }

        private void RemoveFromCache(
            Record record)
        {
            if (!_recordCache.ContainsKey(record.Id)) return;

            _recordCache.Remove(record.Id);
        }

        private void RemoveFromCache(
            RecordAssociation recordAssociation)
        {
            if (!_recordAssociationCache.ContainsKey(recordAssociation.Id)) return;

            _recordAssociationCache[recordAssociation.Id].DecoupleFromRecords();
            _recordAssociationCache.Remove(recordAssociation.Id);
        }
    }
}
