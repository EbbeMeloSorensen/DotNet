using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Craft.Persistence;
using Glossary.Domain.Entities;

namespace Glossary.Persistence.Repositories
{
    public interface IRecordRepository : IRepository<Record>
    {
        Record Get(
            Guid id);

        Record GetRecordIncludingAssociations(
            Guid id);

        IList<Record> GetRecordsIncludingAssociations(
            Expression<Func<Record, bool>> predicate);
    }
}
