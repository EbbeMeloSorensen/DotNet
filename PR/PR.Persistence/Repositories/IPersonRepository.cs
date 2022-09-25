using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Craft.Persistence;
using PR.Domain.Entities;

namespace PR.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        Person Get(
            Guid id);

        Person GetPersonIncludingAssociations(
            Guid id);

        IList<Person> GetPeopleIncludingAssociations(
            Expression<Func<Person, bool>> predicate);
    }
}
