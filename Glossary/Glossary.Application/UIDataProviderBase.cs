using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using Glossary.IO;
using Glossary.Persistence;
using Record = Glossary.Domain.Entities.Record;
using RecordAssociation = Glossary.Domain.Entities.RecordAssociation;

namespace Glossary.Application
{
    public abstract class UIDataProviderBase : IUIDataProvider
    {
        protected ILogger _logger;
        private readonly IDataIOHandler _dataIOHandler;

        public abstract IUnitOfWorkFactory UnitOfWorkFactory { get; }

        protected UIDataProviderBase(
            IDataIOHandler dataIOHandler)
        {
            _dataIOHandler = dataIOHandler;
        }

        public virtual void Initialize(
            ILogger logger)
        {
            _logger = logger;
        }

        public abstract Task<bool> CheckConnection();
        public int CountAllRecords()
        {
            throw new NotImplementedException();
        }

        public abstract void CreateRecord(Record record);

        public abstract void CreateRecordAssociation(
            RecordAssociation recordAssociation);

        public abstract int CountRecords(
            Expression<Func<Record, bool>> predicate);

        public abstract Record GetRecord(
            Guid id);

        public abstract Record GetRecordWithAssociations(
            Guid id);

        public abstract IList<Record> GetAllRecords();

        public abstract IList<RecordAssociation> GetAllRecordAssociations();

        public abstract IList<Record> FindRecords(
            Expression<Func<Record, bool>> predicate);

        public abstract IList<Record> FindRecords(
            IList<Expression<Func<Record, bool>>> predicates);

        public abstract void UpdateRecord(Record record);

        public abstract void UpdateRecords(IList<Record> records);

        public abstract void UpdateRecordAssociation(RecordAssociation recordAssociation);

        public abstract void DeleteRecord(Record record);

        public abstract void DeleteRecords(IList<Record> record);

        public abstract void DeleteRecordAssociations(
            IList<RecordAssociation> recordAssociations);

        public void ExportData(
            string fileName,
            IList<Expression<Func<Record, bool>>> predicates)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            IList<Record> records;
            IList<RecordAssociation> recordAssociations;

            if (predicates == null || predicates.Count == 0)
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving all records from repository..");
                records = GetAllRecords();
                recordAssociations = GetAllRecordAssociations();
            }
            else
            {
                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving matching records from repository..");
                records = FindRecords(predicates);

                // Todo: Handle record associtations
                throw new NotImplementedException();
            }

            _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieved {records.Count} records");

            var glossaryData = new GlossaryData
            {
                Records = records.ToList(),
                RecordAssociations = recordAssociations.ToList()
            };

            switch (extension)
            {
                case ".xml":
                    {
                        _dataIOHandler.ExportDataToXML(glossaryData, fileName);
                        _logger?.WriteLine(LogMessageCategory.Information,
                            $"  Exported {records.Count} records to xml file");
                        break;
                    }
                case ".json":
                    {
                        _dataIOHandler.ExportDataToJson(glossaryData, fileName);
                        _logger?.WriteLine(LogMessageCategory.Information,
                            $"  Exported {records.Count} records to json file");
                        break;
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }

        public void ImportData(
            string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();

            if (extension == null)
            {
                throw new ArgumentException();
            }

            var glossaryData = new GlossaryData();

            switch (extension)
            {
                case ".xml":
                {
                    _dataIOHandler.ImportDataFromXML(
                        fileName, out glossaryData);
                    break;
                }
                case ".json":
                {
                    _dataIOHandler.ImportDataFromJson(
                        fileName, out glossaryData);
                    break;
                }
                default:
                {
                    throw new ArgumentException();
                }
            }

            LoadRecords(glossaryData.Records);
            LoadRecordAssociations(glossaryData.RecordAssociations);
        }

        public event EventHandler<RecordEventArgs> RecordCreated;
        public event EventHandler<RecordsEventArgs> RecordsUpdated;
        public event EventHandler<RecordsEventArgs> RecordsDeleted;

        protected virtual void OnRecordCreated(
            Record record)
        {
            var handler = RecordCreated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new RecordEventArgs(record));
            }
        }

        protected virtual void OnRecordsUpdated(
            IEnumerable<Record> records)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = RecordsUpdated;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new RecordsEventArgs(records));
            }
        }

        protected virtual void OnRecordsDeleted(
            IEnumerable<Record> records)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = RecordsDeleted;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, new RecordsEventArgs(records));
            }
        }

        protected abstract void LoadRecords(
            IList<Record> records);

        protected abstract void LoadRecordAssociations(
            IList<RecordAssociation> recordAssociations);
    }
}
