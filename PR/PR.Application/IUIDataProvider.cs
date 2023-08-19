using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Craft.Logging;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.Application
{
    public interface IUIDataProvider
    {
        IUnitOfWorkFactory UnitOfWorkFactory { get; }

        void Initialize(ILogger logger);

        void CreatePerson(
            Person person);

        void CreatePersonAssociation(
            PersonAssociation personAssociation);

        Person GetPersonWithAssociations(Guid id);

        IList<Person> FindPeople(
            Expression<Func<Person, bool>> predicate);

        IList<Person> FindPeople(
            IList<Expression<Func<Person, bool>>> predicates);

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

        void ExportDataToGraphML(
            IList<Person> people,
            IList<PersonAssociation> personAssociations);

        void ImportData(
            string fileName,
            bool legacy);

        event EventHandler<PersonEventArgs> PersonCreated;
        event EventHandler<PeopleEventArgs> PeopleUpdated;
        event EventHandler<PeopleEventArgs> PeopleDeleted;
    }
}
