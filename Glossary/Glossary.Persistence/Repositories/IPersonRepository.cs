using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Craft.Persistence;
using Glossary.Domain.Entities;

namespace Glossary.Persistence.Repositories
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
