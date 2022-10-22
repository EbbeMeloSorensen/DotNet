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
            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();

            return unitOfWork.Records.Count(predicate);
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
            var records = new List<Record>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.Records.GetAll().ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    records.Add(cacheStationInformation);
                });
            }

            return records;
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
            var records = new List<Record>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var recordsFromRepository = unitOfWork.Records.Find(predicate).ToList();

                recordsFromRepository.ForEach(p =>
                {
                    var cacheRecord = IncludeInCache(p);
                    records.Add(cacheRecord);
                });
            }

            return records;
        }

        public override IList<Record> FindRecords(
            IList<Expression<Func<Record, bool>>> predicates)
        {
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

            // Update the records of the cache too
            foreach (var record in records)
            {
                var cacheObj = GetRecord(record.Id);
                cacheObj.CopyAttributes(record);
            }

            OnRecordsUpdated(records);
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
            IList<Record> records)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var ids = records.Select(p => p.Id).ToList();

                var recordsForDeletion = unitOfWork.Records
                    .GetRecordsIncludingAssociations(p => ids.Contains(p.Id))
                    .ToList();

                var recordAssociationsForDeletion = recordsForDeletion
                    .SelectMany(p => p.ObjectRecords)
                    .Concat(recordsForDeletion.SelectMany(p => p.SubjectRecords))
                    .ToList();

                unitOfWork.RecordAssociations.RemoveRange(recordAssociationsForDeletion);
                unitOfWork.Records.RemoveRange(recordsForDeletion);
                unitOfWork.Complete();

                recordAssociationsForDeletion.ForEach(RemoveFromCache);
                recordsForDeletion.ForEach(RemoveFromCache);
            }

            OnRecordsDeleted(records);
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
            IList<Record> records)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Records.AddRange(records);
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
