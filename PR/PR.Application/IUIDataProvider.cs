using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.Application
{
    public interface IUIDataProvider
    {
        IUnitOfWorkFactory UnitOfWorkFactory { get; }

        void DeletePeople(
            IList<Person> people);

        void ExportData(
            string fileName,
            IList<Expression<Func<Person, bool>>> predicates);

        void ExportDataToGraphML(
            IList<Person> people,
            IList<PersonAssociation> personAssociations);

        void ImportData(
            string fileName,
            bool legacy);

        event EventHandler<PeopleEventArgs> PeopleDeleted;
    }
}
