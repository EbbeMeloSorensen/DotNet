using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Craft.Persistence;
using Glossary.Domain.Entities;

namespace Glossary.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Record>
    {
        Record Get(
            Guid id);

        Record GetPersonIncludingAssociations(
            Guid id);

        IList<Record> GetPeopleIncludingAssociations(
            Expression<Func<Record, bool>> predicate);
    }
}
