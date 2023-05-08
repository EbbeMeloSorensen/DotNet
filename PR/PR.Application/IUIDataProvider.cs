using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.Application
{
    public interface IUIDataProvider
    {
        IUnitOfWorkFactory UnitOfWorkFactory { get; }

        void Initialize(ILogger logger);

        Task<bool> CheckConnection();

        int CountAllPeople();

        void CreatePerson(
            Person person);

        void CreatePersonAssociation(
            PersonAssociation personAssociation);

        int CountPeople(
            Expression<Func<Person, bool>> predicate);

        Person GetPerson(Guid id);

        Person GetPersonWithAssociations(Guid id);

        IList<Person> GetAllPeople();

        IList<Person> FindPeople(
            Expression<Func<Person, bool>> predicate);

        IList<Person> FindPeople(
            IList<Expression<Func<Person, bool>>> predicates);

        IList<PersonAssociation> FindPersonAssociations(
            Expression<Func<PersonAssociation, bool>> predicate);

        IList<PersonAssociation> FindPersonAssociations(
            IList<Expression<Func<PersonAssociation, bool>>> predicates);

        void UpdatePerson(
            Person person);

        void UpdatePeople(
            IList<Person> people);

        void UpdatePersonAssociation(
            PersonAssociation personAssociation);

        void DeletePerson(
            Person person);

        void DeletePeople(
            IList<Person> people);

        void DeletePersonAssociations(
            IList<PersonAssociation> personAssociations);

        void ExportData(
            string fileName,
            IList<Expression<Func<Person, bool>>> predicates);

        void ImportData(
            string fileName,
            bool legacy);

        event EventHandler<PersonEventArgs> PersonCreated;
        event EventHandler<PeopleEventArgs> PeopleUpdated;
        event EventHandler<PeopleEventArgs> PeopleDeleted;
    }
}
