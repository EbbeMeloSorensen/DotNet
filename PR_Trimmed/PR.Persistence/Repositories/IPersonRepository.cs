using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Persistence;
using PR.Domain.Entities.PR;

namespace PR.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        ILogger Logger { get; }

        Task<Person> Get(
            Guid id);

        Task<IEnumerable<Person>> GetAllVariants(
            Guid id);

        Task<IEnumerable<DateTime>> GetAllValidTimeIntervalExtrema();

        Task<IEnumerable<DateTime>> GetAllDatabaseWriteTimes();
    }
}
