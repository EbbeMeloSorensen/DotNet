using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using Glossary.Domain.Entities;
using Glossary.Persistence;

namespace Glossary.Application
{
    public interface IUIDataProvider
    {
        IUnitOfWorkFactory UnitOfWorkFactory { get; }

        void Initialize(ILogger logger);

        Task<bool> CheckConnection();

        int CountAllRecords();

        void CreateRecord(
            Record record);

        void CreateRecordAssociation(
            RecordAssociation recordAssociation);

        int CountRecords(
            Expression<Func<Record, bool>> predicate);

        Record GetRecord(Guid id);

        Record GetRecordWithAssociations(Guid id);

        IList<Record> GetAllRecords();

        IList<Record> FindRecords(
            Expression<Func<Record, bool>> predicate);

        IList<Record> FindRecords(
            IList<Expression<Func<Record, bool>>> predicates);

        void UpdateRecord(
            Record record);

        void UpdateRecords(
            IList<Record> records);

        void UpdateRecordAssociation(
            RecordAssociation recordAssociation);

        void DeleteRecord(
            Record record);

        void DeleteRecords(
            IList<Record> records);

        void DeleteRecordAssociations(
            IList<RecordAssociation> recordAssociations);

        void ExportData(
            string fileName,
            IList<Expression<Func<Record, bool>>> predicates);

        void ImportData(
            string fileName);

        event EventHandler<RecordEventArgs> RecordCreated;
        event EventHandler<RecordsEventArgs> RecordsUpdated;
        event EventHandler<RecordsEventArgs> RecordsDeleted;
    }
}
