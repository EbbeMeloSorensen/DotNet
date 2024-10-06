using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Craft.Persistence;
using PR.Domain.Entities;

namespace PR.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        // Dette er en almindelig Get, der trækker en række med et givet primary key id
        Person Get(
            Guid id);

        Person GetPersonIncludingAssociations(
            Guid id);

        IList<Person> GetPeopleIncludingAssociations(
            Expression<Func<Person, bool>> predicate);

        Person GetObject(
            Guid objectId,
            DateTime? databaseTime = null);

        IEnumerable<Person> GetAllObjects(
            DateTime? databaseTime = null);

        Person GetObjectIncludingPersonAssociations(
            Guid objectId,
            DateTime? databaseTime = null);
    }
}
