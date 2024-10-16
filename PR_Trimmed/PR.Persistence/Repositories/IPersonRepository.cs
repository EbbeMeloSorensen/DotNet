using System;
using System.Threading.Tasks;
using Craft.Persistence;
using PR.Domain.Entities;

namespace PR.Persistence.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<Person> Get(
            Guid id);
    }
}
