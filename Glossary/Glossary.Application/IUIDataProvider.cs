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

        int CountAllPeople();

        void CreateRecord(
            Record record);

        void CreateRecordAssociation(
            RecordAssociation recordAssociation);

        int CountPeople(
            Expression<Func<Record, bool>> predicate);

        Record GetRecord(Guid id);

        Record GetPersonWithAssociations(Guid id);

        IList<Record> GetAllPeople();

        IList<Record> FindPeople(
            Expression<Func<Record, bool>> predicate);

        IList<Record> FindPeople(
            IList<Expression<Func<Record, bool>>> predicates);

        void UpdatePerson(
            Record person);

        void UpdatePeople(
            IList<Record> people);

        void UpdatePersonAssociation(
            RecordAssociation personAssociation);

        void DeletePerson(
            Record person);

        void DeletePeople(
            IList<Record> people);

        void DeletePersonAssociations(
            IList<RecordAssociation> personAssociations);

        void ExportData(
            string fileName,
            IList<Expression<Func<Record, bool>>> predicates);

        void ImportData(
            string fileName,
            bool legacy);

        event EventHandler<PersonEventArgs> PersonCreated;
        event EventHandler<PeopleEventArgs> PeopleUpdated;
        event EventHandler<PeopleEventArgs> PeopleDeleted;
    }
}
